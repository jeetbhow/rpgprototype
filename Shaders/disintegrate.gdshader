shader_type particles;
render_mode keep_data, disable_velocity;

/* ———  TEXTURE & PLACEMENT  ——— */
uniform sampler2D src_tex;
uniform vec2  tex_size      = vec2(256.0);
uniform float pixel_size    = 1.0;
uniform vec2  origin        = vec2(0.0);

/* ———  DISINTEGRATION TWEAKS  ——— */
uniform float max_delay        = 0.25;            // 0 … this many seconds
uniform float rise_speed_min   = 20.0;            // units / s (vertical)
uniform float rise_speed_max   = 40.0;
uniform float side_speed_range = 12.0;            // ± range/2 units / s (horizontal)
uniform float fade_time        = 0.50;            // how long the pixel takes to fade out

/* CUSTOM layout
   x  vertical speed   (units / s, positive value)
   y  horizontal speed (units / s, can be − or +)
   z  launch_time  (absolute TIME at which we start moving)
   w  death_time   (absolute TIME at which we turn ACTIVE off)    */
void start() {
    int idx = int(INDEX);
    int w   = int(tex_size.x);
    int x   = idx % w;
    int y   = idx / w;

    /* 1 ─ colour -------------------------------------------------- */
    vec2 uv  = (vec2(float(x), float(y)) + vec2(0.5)) / tex_size;
    vec4 col = texture(src_tex, uv);
    COLOR    = col;

    /* 2 ─ initial position (upright in 2-D) ----------------------- */
	vec3 local_pos = vec3(
    origin.x + float(x) * pixel_size,
    origin.y + float(y) * pixel_size,
    0.0);
	TRANSFORM[3].xyz = (EMISSION_TRANSFORM * vec4(local_pos, 1.0)).xyz;

    /* 3 ─ default state ------------------------------------------ */
    ACTIVE = true;

    /* 4 ─ skip transparent pixels without early-return ----------- */
    if (col.a < 0.05) {
        ACTIVE = false;
    } else {
        /* pseudo-random numbers (deterministic per pixel) */
        uint seed = uint(INDEX) ^ RANDOM_SEED;

        seed  = seed * 1664525u + 1013904223u;   float r1 = float(seed & 0x00FFFFFFu) / 16777215.0;
        seed  = seed * 1664525u + 1013904223u;   float r2 = float(seed & 0x00FFFFFFu) / 16777215.0;
        seed  = seed * 1664525u + 1013904223u;   float r3 = float(seed & 0x00FFFFFFu) / 16777215.0;

        /* motion parameters */
        CUSTOM.x = mix(rise_speed_min, rise_speed_max, r1);        // upward speed
        CUSTOM.y = (r2 - 0.5) * side_speed_range;                  // sideways jitter

        float delay       = r3 * max_delay;                        // 0 … max_delay
        CUSTOM.z = TIME + delay;                                   // **absolute** launch time
        CUSTOM.w = CUSTOM.z + fade_time;                           // **absolute** death time
    }
}

void process() {
    if (ACTIVE) {
        /* start moving once the personal delay has passed */
        if (TIME >= CUSTOM.z) {
            TRANSFORM[3].y -= CUSTOM.x * DELTA;       // rise (screen-upwards)
            TRANSFORM[3].x += CUSTOM.y * DELTA;       // sideways drift

            /* fade from opaque → transparent over fade_time seconds */
            float t = clamp((TIME - CUSTOM.z) / fade_time, 0.0, 1.0);
            COLOR.a = mix(1.0, 0.0, t);
        }

        /* kill the particle after it has faded out */
        if (TIME >= CUSTOM.w) {
            ACTIVE = false;
        }
    }
}

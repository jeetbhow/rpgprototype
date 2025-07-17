// res://shaders/blink.shader
shader_type canvas_item;

uniform float start_time = -1.0;			// To be passed in by the main script.

uniform bool blinking = false;
uniform float blink_initial_speed = 50.0;
uniform float blink_min_speed = 1.0;
uniform float blink_decay_rate = 2.0;

uniform bool shaking = false;
uniform float shake_progress = 1.0;
uniform float shake_intensity = 4.0;
uniform float initial_shake_speed = 80.0;
uniform float shake_duration = 1.0;

void vertex() {
	float elapsed = TIME - start_time;

	if (shaking && elapsed < shake_duration) {
        float progress = elapsed / shake_duration;
        float shake_speed = mix(initial_shake_speed, 0.0, progress);
        float x_offset = sin(elapsed * shake_speed) * shake_intensity * (1.0 - progress);
        VERTEX.x += x_offset;
	}
}

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    if (blinking) {
        // compute the *integral* of speed(t) = min + (init–min)*e^(–decay·t)
        // ∫ speed(t) dt = min_speed * TIME
        //            + (blink_initial_speed – blink_min_speed) * (1 – e^(–decay·TIME)) / blink_decay_rate
        float decayed  = exp(-blink_decay_rate * (TIME - start_time));
        float integral = blink_min_speed * (TIME - start_time)
                       + (blink_initial_speed - blink_min_speed) * (1.0 - decayed) / blink_decay_rate;

        // use fract(integral) as our phase, then square‐wave it at 50% duty
        float on = step(0.5, fract(integral));
        tex.a *= on;
		COLOR = tex;
    }
}

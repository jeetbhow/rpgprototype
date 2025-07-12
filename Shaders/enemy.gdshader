// res://shaders/blink.shader
shader_type canvas_item;

// starting (fast) blinks per second
uniform float blink_initial_speed = 50.0;
// asymptotic minimum blinks per second
uniform float blink_min_speed = 1.0;
// how quickly it slows (per second)
uniform float blink_decay_rate = 2.0;
uniform float elapsed_time = -1.0;			// To be passed in by the main script.

uniform bool blinking = false;

void fragment() {
    vec4 tex = texture(TEXTURE, UV);

    if (blinking) {
        // compute the *integral* of speed(t) = min + (init–min)*e^(–decay·t)
        // ∫ speed(t) dt = min_speed * TIME
        //            + (blink_initial_speed – blink_min_speed) * (1 – e^(–decay·TIME)) / blink_decay_rate
        float decayed  = exp(-blink_decay_rate * (TIME - elapsed_time));
        float integral = blink_min_speed * (TIME - elapsed_time)
                       + (blink_initial_speed - blink_min_speed) * (1.0 - decayed) / blink_decay_rate;

        // use fract(integral) as our phase, then square‐wave it at 50% duty
        float on = step(0.5, fract(integral));
        tex.a *= on;
    }

    COLOR = tex;
}

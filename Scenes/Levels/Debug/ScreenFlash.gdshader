shader_type canvas_item;
render_mode blend_mix, unshaded;  // allows transparency blending

uniform float flash_strength : hint_range(0.0, 1.0) = 0.0;
uniform vec4 flash_color = vec4(1.0, 0.0, 0.0, 1.0); // red by default

void fragment() {
    // Only output flash color with strength
    COLOR = vec4(flash_color.rgb, flash_color.a * flash_strength);
}

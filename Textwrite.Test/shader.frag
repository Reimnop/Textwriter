#version 430 core

layout(location = 0) out vec4 fragColor;

layout(location = 0) in vec2 UV;
layout(location = 1) in vec4 Color;

layout(location = 1) uniform sampler2D atlas;

void main() {
    fragColor = vec4(texture(atlas, UV).r) * Color;
}

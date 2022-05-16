#version 430 core

layout(location = 0) out vec4 fragColor;

layout(location = 0) in vec2 UV;
layout(location = 1) in vec4 Color;
layout(location = 2) flat in int Index;

layout(location = 1) uniform sampler2D atlas[4];

void main() {
    float a = Index >= 0 ? texture(atlas[Index], UV).r : 1.0;
    fragColor = vec4(1.0, 1.0, 1.0, a) * Color;
}

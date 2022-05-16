#version 430 core

layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aUv;
layout(location = 2) in vec4 aColor;
layout(location = 3) in int aIndex;

layout(location = 0) out vec2 UV;
layout(location = 1) out vec4 Color;
layout(location = 2) flat out int Index;

layout(location = 0) uniform mat4 mvp;

void main() {
    UV = aUv;
    Color = aColor;
    Index = aIndex;
    gl_Position = vec4(aPos, 0.0, 1.0) * mvp;
}
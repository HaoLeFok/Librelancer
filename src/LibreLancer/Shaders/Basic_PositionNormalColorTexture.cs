﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibreLancer.Shaders
{
    using System;
    
    public class Basic_PositionNormalColorTexture
    {
        private static byte[] vertex_bytes = new byte[2085] {
27, 105, 39, 0, 156, 7, 182, 13, 105, 119, 17, 58, 170, 242, 217, 91, 232, 38, 232, 231, 111, 25, 74, 186, 141, 226, 40, 79, 178, 163,
69, 117, 20, 77, 38, 247, 253, 119, 169, 117, 231, 114, 122, 123, 218, 143, 75, 223, 102, 196, 148, 218, 44, 98, 67, 6, 165, 56, 178, 73,
214, 153, 73, 16, 133, 24, 176, 244, 128, 248, 51, 176, 201, 181, 245, 251, 159, 251, 180, 64, 178, 40, 69, 207, 186, 74, 249, 231, 78, 222,
59, 155, 124, 232, 98, 22, 8, 11, 248, 102, 242, 126, 78, 55, 155, 34, 168, 42, 2, 5, 168, 106, 73, 170, 106, 185, 160, 186, 12, 85,
147, 180, 94, 50, 218, 111, 163, 7, 18, 2, 217, 244, 23, 163, 24, 63, 196, 195, 254, 205, 221, 254, 83, 117, 123, 180, 189, 151, 218, 89,
32, 13, 33, 101, 16, 39, 206, 183, 111, 248, 134, 129, 23, 111, 241, 143, 249, 112, 105, 201, 174, 116, 93, 124, 73, 106, 103, 98, 84, 69,
224, 154, 135, 96, 202, 194, 54, 132, 240, 9, 17, 144, 45, 229, 120, 48, 168, 228, 167, 138, 208, 99, 56, 104, 199, 251, 96, 18, 111, 199,
196, 10, 251, 236, 75, 115, 103, 167, 186, 127, 194, 168, 153, 34, 195, 230, 254, 170, 237, 105, 224, 87, 154, 47, 21, 190, 110, 122, 158, 199,
175, 119, 14, 141, 73, 88, 152, 167, 229, 129, 244, 100, 248, 164, 136, 235, 182, 157, 211, 243, 165, 217, 251, 45, 239, 153, 189, 71, 172, 191,
176, 31, 111, 207, 118, 88, 45, 229, 52, 231, 14, 122, 70, 113, 243, 245, 184, 141, 133, 213, 46, 223, 91, 104, 199, 4, 80, 11, 89, 2,
184, 125, 123, 230, 202, 35, 87, 25, 91, 69, 54, 169, 41, 33, 171, 46, 67, 173, 93, 44, 242, 24, 211, 104, 243, 43, 105, 125, 176, 200,
19, 203, 27, 188, 184, 63, 103, 255, 171, 234, 106, 255, 166, 186, 59, 62, 219, 199, 175, 52, 99, 254, 201, 237, 241, 37, 173, 33, 237, 56,
190, 153, 241, 53, 118, 93, 18, 219, 42, 181, 146, 204, 83, 69, 105, 192, 2, 194, 5, 78, 139, 214, 65, 61, 248, 156, 89, 152, 113, 71,
215, 254, 153, 187, 163, 111, 12, 121, 161, 132, 219, 79, 213, 232, 110, 255, 222, 230, 209, 210, 24, 221, 161, 169, 21, 89, 83, 220, 76, 189,
107, 138, 60, 249, 178, 115, 99, 146, 132, 31, 196, 163, 102, 104, 240, 80, 136, 104, 103, 157, 216, 185, 5, 163, 166, 74, 189, 212, 125, 139,
41, 220, 109, 138, 56, 180, 156, 154, 210, 128, 104, 235, 49, 236, 125, 181, 238, 249, 137, 162, 186, 79, 185, 86, 120, 163, 162, 56, 239, 62,
104, 152, 18, 140, 140, 200, 173, 183, 193, 39, 65, 57, 231, 245, 248, 176, 83, 139, 207, 119, 232, 191, 31, 62, 50, 248, 140, 62, 6, 248,
212, 140, 20, 187, 166, 173, 114, 48, 125, 212, 134, 78, 57, 91, 211, 198, 246, 33, 48, 149, 206, 28, 93, 201, 34, 103, 63, 51, 55, 222,
63, 192, 123, 203, 185, 97, 68, 205, 221, 84, 163, 185, 132, 117, 190, 142, 183, 104, 191, 60, 252, 110, 173, 75, 213, 111, 93, 58, 59, 79,
47, 220, 127, 186, 66, 122, 155, 207, 144, 49, 23, 8, 110, 124, 190, 50, 237, 152, 140, 134, 220, 107, 185, 50, 183, 64, 102, 86, 127, 84,
25, 29, 36, 231, 34, 21, 240, 248, 51, 203, 149, 197, 214, 141, 141, 237, 29, 36, 174, 122, 43, 145, 107, 229, 128, 67, 99, 200, 191, 221,
32, 91, 162, 144, 107, 109, 200, 164, 54, 180, 9, 33, 67, 169, 59, 43, 228, 90, 110, 100, 12, 231, 70, 9, 98, 244, 176, 32, 142, 221,
30, 136, 237, 17, 124, 6, 22, 58, 160, 247, 225, 119, 13, 178, 37, 105, 121, 20, 60, 114, 28, 65, 146, 121, 209, 39, 0, 120, 175, 117,
201, 168, 142, 97, 133, 40, 141, 137, 144, 151, 135, 112, 196, 57, 106, 129, 183, 58, 145, 83, 234, 169, 154, 45, 104, 235, 154, 64, 18, 37,
65, 57, 98, 46, 172, 109, 106, 102, 7, 242, 96, 207, 70, 127, 60, 10, 52, 28, 7, 40, 68, 252, 142, 93, 153, 233, 74, 205, 247, 105,
110, 179, 100, 15, 59, 240, 46, 157, 205, 156, 213, 88, 99, 150, 189, 174, 58, 236, 168, 234, 206, 243, 8, 0, 234, 175, 219, 69, 97, 16,
124, 193, 186, 106, 165, 59, 71, 176, 194, 14, 125, 108, 147, 187, 127, 178, 202, 1, 96, 51, 74, 194, 124, 59, 32, 47, 227, 180, 71, 193,
155, 155, 113, 1, 10, 9, 188, 211, 224, 28, 146, 207, 132, 253, 210, 180, 38, 97, 12, 149, 206, 240, 201, 25, 187, 218, 142, 124, 170, 19,
152, 57, 253, 76, 40, 166, 221, 0, 150, 133, 254, 87, 128, 49, 100, 177, 209, 35, 132, 136, 79, 158, 96, 162, 246, 18, 140, 149, 58, 238,
90, 96, 7, 0, 109, 79, 56, 127, 148, 223, 27, 8, 114, 59, 41, 150, 6, 7, 52, 194, 31, 27, 108, 255, 91, 35, 128, 106, 92, 140,
56, 41, 231, 90, 172, 83, 211, 64, 90, 126, 251, 53, 165, 108, 13, 252, 158, 174, 151, 221, 74, 151, 52, 6, 177, 58, 68, 150, 183, 203,
182, 208, 9, 142, 111, 64, 90, 38, 238, 141, 5, 184, 161, 132, 78, 189, 58, 201, 105, 65, 5, 9, 173, 170, 164, 10, 51, 67, 74, 55,
114, 129, 174, 215, 227, 205, 33, 171, 14, 94, 193, 65, 53, 77, 93, 45, 6, 170, 130, 11, 144, 138, 132, 234, 125, 140, 176, 17, 104, 178,
58, 222, 54, 86, 115, 122, 214, 241, 166, 245, 12, 219, 130, 174, 110, 5, 57, 132, 171, 88, 20, 221, 152, 42, 137, 119, 140, 136, 6, 250,
44, 94, 123, 184, 240, 228, 51, 249, 181, 101, 210, 162, 163, 228, 228, 188, 107, 82, 209, 177, 170, 228, 222, 51, 102, 166, 6, 26, 193, 194,
228, 230, 131, 35, 30, 72, 82, 165, 230, 225, 38, 185, 172, 165, 128, 247, 205, 2, 22, 92, 12, 61, 235, 128, 88, 123, 32, 131, 32, 69,
208, 244, 227, 197, 214, 228, 219, 112, 141, 119, 208, 198, 186, 208, 106, 55, 54, 218, 206, 79, 98, 29, 187, 159, 61, 20, 122, 80, 166, 161,
215, 223, 250, 139, 166, 117, 103, 144, 53, 84, 88, 189, 159, 230, 193, 184, 187, 223, 114, 222, 87, 216, 186, 145, 82, 0, 221, 178, 101, 168,
100, 62, 107, 186, 207, 22, 18, 14, 138, 161, 63, 16, 8, 249, 9, 78, 254, 154, 216, 232, 181, 69, 251, 159, 122, 181, 144, 56, 220, 42,
164, 74, 162, 122, 201, 164, 204, 92, 226, 144, 167, 39, 131, 216, 86, 5, 11, 17, 181, 118, 158, 134, 236, 240, 108, 73, 148, 188, 213, 99,
140, 194, 234, 8, 27, 211, 84, 20, 240, 118, 143, 182, 192, 117, 143, 77, 16, 189, 250, 100, 182, 200, 181, 14, 105, 252, 224, 206, 148, 85,
250, 91, 51, 200, 137, 119, 219, 61, 162, 178, 253, 41, 85, 175, 62, 201, 129, 210, 45, 33, 41, 110, 227, 69, 233, 236, 239, 45, 33, 57,
20, 186, 165, 223, 109, 204, 16, 207, 45, 74, 215, 14, 170, 176, 28, 188, 144, 244, 177, 86, 16, 77, 55, 223, 212, 202, 9, 82, 55, 135,
36, 156, 78, 73, 193, 213, 38, 105, 158, 130, 112, 44, 210, 101, 210, 24, 245, 6, 200, 106, 203, 22, 144, 198, 112, 28, 174, 52, 66, 230,
168, 209, 12, 136, 47, 58, 184, 217, 62, 60, 223, 191, 184, 187, 222, 222, 204, 239, 69, 190, 126, 119, 84, 215, 69, 68, 171, 227, 170, 14,
252, 107, 210, 77, 205, 27, 112, 122, 8, 12, 152, 105, 87, 165, 182, 39, 203, 81, 117, 16, 46, 140, 53, 53, 60, 62, 234, 216, 57, 134,
220, 61, 98, 34, 137, 57, 120, 168, 122, 165, 44, 56, 140, 141, 167, 213, 65, 121, 75, 7, 63, 202, 178, 112, 75, 160, 67, 74, 203, 166,
218, 129, 109, 130, 22, 12, 70, 52, 108, 127, 2, 72, 122, 170, 56, 146, 141, 143, 56, 208, 161, 208, 136, 239, 31, 1, 132, 185, 243, 144,
144, 166, 198, 117, 0, 186, 147, 240, 161, 245, 78, 120, 79, 1, 13, 39, 178, 37, 23, 44, 254, 41, 132, 69, 180, 45, 117, 36, 97, 101,
1, 88, 25, 36, 229, 113, 5, 69, 129, 104, 55, 28, 55, 68, 170, 172, 228, 216, 108, 39, 132, 88, 147, 210, 94, 251, 139, 96, 168, 206,
254, 251, 149, 204, 44, 186, 81, 154, 75, 132, 165, 27, 180, 195, 54, 228, 64, 117, 58, 220, 255, 195, 59, 96, 161, 172, 186, 166, 184, 167,
99, 155, 168, 216, 238, 29, 125, 212, 103, 223, 178, 211, 87, 103, 79, 62, 102, 127, 149, 174, 182, 209, 139, 44, 91, 88, 30, 240, 22, 48,
92, 78, 131, 29, 93, 108, 193, 110, 156, 61, 191, 41, 50, 211, 106, 155, 165, 111, 176, 203, 173, 208, 15, 62, 35, 92, 246, 132, 72, 120,
239, 141, 113, 33, 229, 142, 63, 239, 19, 67, 89, 81, 51, 251, 143, 192, 118, 135, 132, 180, 251, 190, 0, 117, 143, 141, 120, 60, 131, 157,
61, 112, 225, 81, 93, 58, 1, 129, 22, 11, 200, 80, 199, 22, 48, 233, 240, 156, 177, 42, 229, 176, 234, 216, 6, 97, 89, 23, 131, 224,
40, 159, 157, 97, 81, 148, 27, 185, 99, 253, 103, 53, 222, 109, 219, 174, 137, 88, 115, 129, 240, 1, 103, 167, 250, 120, 15, 18, 179, 49,
30, 169, 223, 129, 183, 193, 165, 242, 29, 134, 235, 130, 196, 131, 219, 209, 36, 42, 251, 249, 151, 13, 168, 60, 103, 190, 36, 105, 228, 242,
14, 48, 218, 125, 212, 15, 7, 45, 175, 201, 88, 52, 19, 194, 239, 136, 89, 20, 29, 85, 11, 4, 168, 241, 59, 223, 163, 255, 243, 63,
90, 88, 221, 188, 104, 149, 224, 26, 77, 9, 113, 73, 211, 37, 110, 85, 45, 14, 142, 130, 106, 39, 228, 251, 130, 132, 195, 46, 247, 251,
56, 65, 26, 163, 119, 207, 134, 139, 249, 182, 2, 147, 158, 170, 239, 215, 210, 33, 117, 211, 49, 224, 24, 82, 192, 28, 181, 222, 175, 185,
96, 44, 181, 136, 230, 178, 235, 250, 58, 227, 166, 188, 100, 169, 186, 239, 171, 155, 30, 136, 94, 112, 211, 75, 140, 26, 21, 252, 69, 82,
201, 63, 98, 3, 220, 50, 185, 231, 252, 79, 133, 195, 211, 69, 76, 241, 57, 88, 132, 215, 67, 192, 182, 15, 138, 137, 237, 130, 119, 237,
222, 22, 138, 126, 99, 61, 48, 202, 20, 153, 220, 226, 205, 99, 83, 1, 22, 38, 53, 101, 69, 23, 128, 246, 167, 162, 99, 155, 200, 46,
172, 208, 115, 88, 119, 6, 101, 145, 234, 11, 198, 4, 57, 155, 91, 151, 91, 206, 54, 3, 243, 151, 92, 73, 45, 45, 89, 77, 238, 108,
149, 13, 51, 229, 138, 161, 136, 97, 179, 68, 14, 66, 222, 228, 20, 76, 121, 3, 57, 237, 158, 227, 224, 247, 58, 87, 174, 179, 195, 204,
23, 243, 201, 150, 54, 108, 207, 237, 76, 180, 111, 129, 102, 108, 221, 79, 156, 77, 84, 33, 147, 81, 248, 116, 194, 147, 113, 128, 195, 211,
159, 166, 16, 123, 156, 122, 232, 158, 86, 1, 122, 59, 36, 189, 194, 118, 165, 86, 136, 1, 238, 188, 238, 187, 230, 99, 131, 173, 86, 107,
70, 176, 234, 196, 9, 191, 105, 138, 187, 25, 164, 73, 92, 102, 250, 35, 226, 162, 218, 180, 196, 128, 112, 8, 86, 56, 97, 45, 133, 132,
65, 193, 147, 220, 108, 51, 48, 56, 103, 81, 237, 60, 237, 64, 110, 237, 45, 128, 253, 252, 138, 16, 66, 162, 197, 117, 203, 14, 140, 89,
27, 19, 187, 136, 76, 165, 155, 7, 181, 129, 218, 226, 115, 147, 3
};
        private static byte[] fragment_bytes = new byte[2151] {
27, 242, 39, 0, 28, 133, 113, 227, 243, 81, 226, 35, 66, 35, 155, 245, 94, 220, 65, 255, 115, 162, 206, 51, 234, 32, 46, 230, 90, 61,
226, 102, 170, 68, 11, 223, 44, 170, 106, 196, 255, 247, 167, 250, 207, 207, 215, 237, 87, 159, 64, 8, 188, 58, 172, 42, 19, 178, 38, 109,
178, 57, 138, 16, 14, 94, 248, 128, 236, 146, 4, 84, 57, 32, 94, 14, 118, 89, 59, 255, 191, 181, 242, 237, 100, 131, 46, 39, 210, 1,
75, 151, 125, 247, 85, 213, 217, 174, 129, 12, 118, 0, 123, 2, 4, 245, 94, 245, 244, 82, 135, 81, 133, 63, 40, 242, 132, 82, 69, 203,
5, 245, 31, 67, 179, 153, 239, 125, 218, 29, 101, 42, 162, 68, 187, 171, 125, 24, 243, 219, 7, 55, 187, 135, 231, 233, 69, 150, 223, 30,
237, 38, 233, 141, 215, 209, 142, 162, 231, 233, 57, 39, 66, 170, 123, 56, 253, 243, 221, 251, 252, 91, 8, 62, 114, 139, 223, 94, 10, 240,
108, 84, 27, 107, 134, 184, 15, 150, 74, 53, 36, 174, 205, 55, 139, 24, 38, 33, 132, 31, 32, 128, 154, 244, 24, 30, 76, 44, 254, 212,
16, 58, 195, 65, 61, 57, 43, 19, 189, 44, 53, 174, 194, 31, 125, 89, 102, 53, 183, 237, 61, 70, 165, 138, 12, 103, 182, 87, 171, 165,
142, 221, 211, 10, 170, 228, 41, 233, 125, 62, 163, 23, 6, 141, 101, 89, 88, 185, 149, 3, 233, 205, 8, 232, 33, 158, 178, 235, 156, 93,
174, 27, 66, 190, 237, 91, 246, 95, 4, 31, 159, 216, 207, 150, 84, 131, 181, 188, 1, 173, 194, 67, 205, 67, 221, 114, 45, 238, 252, 161,
171, 93, 190, 115, 208, 129, 9, 160, 20, 38, 146, 192, 173, 221, 133, 115, 247, 174, 48, 198, 203, 108, 170, 166, 72, 214, 80, 31, 165, 182,
254, 122, 81, 250, 246, 194, 150, 191, 164, 214, 199, 135, 131, 216, 242, 182, 93, 252, 61, 103, 255, 119, 249, 85, 122, 147, 103, 199, 103, 41,
70, 189, 190, 151, 79, 119, 143, 47, 114, 13, 105, 251, 248, 74, 227, 107, 45, 189, 38, 211, 26, 166, 146, 236, 83, 67, 215, 128, 3, 132,
83, 220, 39, 179, 63, 212, 205, 103, 209, 194, 26, 60, 186, 223, 82, 75, 55, 31, 199, 144, 87, 42, 183, 239, 186, 176, 205, 25, 191, 51,
223, 214, 101, 105, 27, 52, 190, 170, 215, 56, 207, 164, 214, 21, 195, 65, 252, 133, 232, 50, 69, 204, 166, 245, 168, 97, 90, 60, 40, 225,
247, 142, 137, 93, 168, 24, 53, 94, 234, 183, 30, 217, 204, 225, 238, 81, 134, 195, 242, 123, 86, 27, 16, 29, 57, 134, 125, 196, 216, 150,
159, 80, 53, 124, 42, 181, 193, 219, 37, 211, 239, 219, 239, 44, 61, 7, 30, 35, 70, 119, 254, 7, 159, 22, 229, 146, 167, 235, 176, 209,
171, 247, 23, 232, 255, 222, 189, 246, 17, 240, 246, 218, 33, 192, 120, 12, 243, 204, 107, 237, 80, 246, 90, 167, 53, 218, 85, 134, 18, 91,
29, 55, 149, 206, 0, 157, 203, 17, 230, 253, 211, 28, 251, 150, 14, 254, 91, 206, 77, 4, 181, 48, 169, 162, 189, 73, 214, 29, 7, 222,
162, 117, 121, 248, 253, 91, 215, 242, 223, 195, 244, 196, 131, 253, 131, 233, 253, 21, 122, 151, 67, 125, 244, 189, 8, 4, 199, 126, 40, 47,
235, 9, 25, 13, 57, 220, 122, 110, 110, 44, 179, 169, 126, 159, 27, 29, 36, 103, 149, 134, 241, 248, 115, 205, 213, 101, 234, 233, 69, 213,
58, 40, 156, 247, 120, 34, 55, 194, 0, 183, 87, 73, 254, 237, 54, 53, 37, 74, 88, 173, 109, 111, 218, 134, 206, 64, 72, 55, 166, 131,
27, 206, 117, 220, 200, 24, 46, 21, 49, 34, 108, 183, 34, 142, 61, 31, 196, 150, 128, 207, 214, 141, 13, 241, 243, 240, 187, 9, 253, 45,
182, 61, 13, 62, 57, 129, 32, 113, 94, 244, 41, 1, 190, 213, 199, 152, 82, 55, 12, 27, 68, 41, 19, 33, 47, 79, 96, 136, 114, 84,
133, 199, 135, 200, 57, 102, 174, 23, 43, 74, 61, 137, 19, 139, 152, 235, 145, 50, 105, 109, 123, 23, 85, 71, 30, 236, 93, 241, 147, 55,
94, 195, 41, 135, 130, 103, 183, 198, 149, 157, 253, 47, 189, 201, 210, 251, 252, 59, 55, 87, 178, 108, 143, 187, 117, 173, 157, 205, 92, 84,
84, 101, 185, 110, 109, 222, 133, 251, 188, 131, 47, 35, 1, 168, 7, 111, 201, 4, 70, 240, 5, 167, 228, 27, 219, 56, 130, 21, 118, 152,
199, 54, 121, 64, 64, 86, 185, 21, 72, 134, 38, 204, 183, 13, 117, 211, 167, 88, 240, 55, 119, 98, 18, 20, 18, 228, 222, 183, 45, 161,
248, 236, 174, 62, 45, 29, 73, 24, 75, 165, 19, 254, 114, 178, 167, 110, 200, 167, 6, 161, 90, 208, 207, 36, 114, 33, 14, 96, 89, 232,
47, 5, 168, 146, 42, 108, 205, 17, 130, 103, 203, 41, 152, 168, 187, 49, 163, 167, 118, 182, 128, 3, 0, 180, 61, 225, 252, 161, 127, 110,
32, 200, 29, 236, 169, 14, 14, 104, 204, 159, 21, 85, 251, 123, 36, 64, 213, 132, 24, 233, 49, 94, 125, 113, 138, 158, 115, 122, 227, 111,
200, 166, 214, 63, 2, 191, 183, 205, 186, 217, 88, 77, 83, 16, 171, 37, 84, 177, 150, 154, 22, 186, 143, 225, 11, 72, 102, 18, 94, 88,
128, 219, 198, 160, 83, 175, 78, 114, 58, 80, 65, 66, 43, 42, 89, 133, 19, 134, 148, 110, 103, 18, 93, 47, 199, 59, 67, 21, 29, 188,
66, 128, 106, 155, 186, 170, 6, 138, 130, 21, 40, 68, 66, 245, 46, 69, 216, 8, 52, 89, 3, 111, 155, 234, 37, 61, 231, 120, 139, 122,
134, 109, 161, 174, 238, 6, 53, 132, 107, 84, 40, 186, 49, 53, 98, 239, 232, 137, 6, 250, 44, 62, 122, 178, 240, 229, 179, 249, 115, 54,
33, 109, 58, 74, 73, 198, 187, 210, 19, 29, 171, 74, 238, 91, 70, 44, 116, 71, 35, 88, 152, 220, 33, 126, 10, 198, 37, 25, 176, 230,
225, 38, 89, 215, 26, 231, 125, 39, 14, 11, 174, 134, 158, 117, 43, 145, 231, 36, 77, 216, 34, 104, 230, 227, 197, 214, 148, 183, 225, 90,
239, 160, 141, 117, 81, 171, 61, 108, 181, 157, 159, 136, 188, 220, 247, 22, 26, 45, 40, 211, 208, 203, 239, 212, 139, 162, 118, 103, 168, 18,
26, 62, 124, 148, 150, 124, 220, 221, 175, 24, 227, 84, 173, 4, 82, 10, 70, 183, 108, 25, 42, 206, 231, 164, 230, 189, 134, 130, 131, 98,
232, 143, 113, 66, 126, 130, 83, 254, 154, 56, 221, 159, 172, 234, 255, 212, 171, 37, 196, 225, 110, 33, 69, 226, 215, 75, 198, 50, 75, 137,
67, 158, 158, 12, 34, 10, 49, 25, 34, 218, 118, 94, 132, 236, 240, 108, 49, 74, 185, 171, 7, 70, 33, 234, 8, 137, 69, 86, 20, 240,
166, 79, 109, 129, 235, 12, 187, 67, 244, 193, 119, 155, 34, 143, 218, 58, 126, 8, 215, 206, 42, 253, 173, 29, 228, 164, 251, 235, 28, 169,
108, 127, 74, 209, 171, 79, 114, 160, 116, 87, 40, 138, 147, 248, 81, 58, 251, 123, 71, 72, 14, 133, 238, 26, 204, 54, 102, 137, 151, 150,
218, 181, 64, 5, 86, 130, 41, 73, 31, 125, 5, 209, 116, 243, 45, 76, 79, 144, 186, 51, 20, 225, 98, 52, 5, 87, 183, 75, 243, 20,
132, 83, 153, 186, 140, 141, 81, 111, 131, 42, 182, 73, 36, 36, 27, 78, 200, 149, 70, 200, 28, 53, 186, 112, 95, 59, 1, 117, 143, 181,
28, 17, 115, 126, 124, 199, 45, 174, 61, 29, 246, 181, 225, 252, 91, 96, 23, 20, 95, 129, 179, 29, 103, 192, 76, 39, 47, 181, 51, 89,
65, 85, 155, 112, 33, 43, 12, 124, 62, 234, 216, 57, 133, 58, 61, 98, 35, 137, 131, 240, 209, 230, 160, 46, 57, 100, 147, 121, 126, 160,
111, 237, 224, 135, 91, 22, 110, 9, 116, 168, 210, 38, 161, 210, 161, 23, 208, 242, 108, 19, 13, 59, 55, 1, 236, 250, 222, 8, 146, 139,
143, 56, 208, 161, 208, 136, 239, 158, 2, 132, 185, 135, 160, 160, 155, 26, 151, 1, 232, 238, 65, 0, 29, 119, 224, 63, 25, 26, 65, 100,
75, 46, 88, 252, 83, 8, 139, 104, 91, 214, 145, 132, 149, 10, 112, 101, 144, 148, 235, 12, 201, 115, 209, 110, 11, 51, 68, 42, 172, 120,
108, 54, 21, 66, 172, 61, 250, 173, 127, 53, 12, 131, 217, 255, 22, 71, 204, 162, 135, 177, 185, 120, 108, 61, 208, 41, 220, 132, 1, 13,
217, 36, 253, 83, 60, 96, 163, 172, 161, 194, 39, 117, 108, 119, 174, 141, 240, 246, 228, 204, 118, 110, 221, 216, 251, 224, 220, 62, 214, 131,
213, 174, 210, 232, 69, 234, 150, 140, 167, 192, 5, 12, 235, 105, 177, 253, 69, 10, 78, 227, 204, 248, 77, 145, 153, 14, 216, 44, 115, 7,
187, 220, 10, 243, 193, 79, 16, 110, 51, 66, 36, 60, 243, 198, 184, 144, 114, 202, 95, 207, 18, 67, 89, 81, 50, 179, 143, 192, 118, 138,
132, 180, 179, 190, 0, 117, 134, 141, 120, 114, 6, 59, 51, 224, 198, 163, 186, 98, 2, 2, 45, 22, 144, 161, 142, 83, 48, 54, 61, 103,
173, 74, 153, 86, 205, 182, 8, 79, 235, 98, 16, 154, 229, 179, 107, 215, 170, 42, 145, 59, 214, 47, 214, 147, 253, 85, 55, 133, 224, 131,
73, 132, 239, 112, 213, 220, 30, 39, 80, 152, 243, 163, 157, 189, 31, 28, 111, 205, 77, 227, 157, 134, 235, 66, 225, 193, 237, 104, 18, 213,
131, 252, 49, 29, 42, 175, 162, 175, 41, 26, 165, 252, 9, 70, 119, 142, 250, 41, 160, 247, 62, 138, 69, 107, 35, 242, 205, 152, 121, 49,
171, 102, 4, 168, 4, 157, 23, 241, 255, 208, 183, 73, 120, 118, 243, 162, 85, 130, 171, 159, 19, 98, 77, 139, 37, 238, 170, 90, 76, 142,
130, 106, 7, 234, 125, 65, 34, 96, 87, 250, 101, 156, 32, 141, 49, 119, 207, 134, 27, 251, 141, 6, 198, 158, 6, 239, 215, 138, 33, 117,
199, 42, 193, 49, 164, 128, 57, 106, 189, 223, 194, 33, 99, 169, 42, 218, 155, 105, 214, 215, 73, 55, 21, 151, 101, 224, 190, 111, 104, 190,
88, 156, 185, 232, 26, 163, 68, 5, 127, 177, 171, 228, 159, 178, 5, 46, 19, 50, 207, 249, 63, 149, 14, 79, 23, 177, 192, 103, 179, 9,
175, 135, 128, 109, 22, 20, 19, 187, 141, 223, 237, 233, 7, 13, 97, 249, 143, 47, 18, 59, 251, 168, 215, 151, 97, 196, 196, 248, 238, 224,
156, 12, 22, 20, 57, 105, 92, 30, 48, 216, 39, 216, 199, 100, 152, 78, 49, 53, 17, 34, 57, 130, 76, 169, 81, 79, 102, 250, 147, 177,
38, 51, 43, 74, 238, 148, 152, 196, 165, 83, 94, 57, 118, 122, 211, 79, 144, 184, 219, 227, 40, 192, 242, 83, 99, 187, 111, 186, 52, 50,
43, 85, 182, 232, 194, 110, 60, 251, 74, 127, 133, 137, 30, 243, 214, 154, 24, 186, 212, 31, 113, 156, 221, 32, 230, 227, 121, 209, 209, 204,
50, 247, 167, 248, 205, 213, 87, 178, 110, 150, 255, 93, 23, 48, 225, 111, 168, 93, 38, 130, 63, 196, 98, 52, 249, 78, 107, 16, 183, 234,
166, 152, 193, 54, 21, 197, 204, 51, 107, 160, 212, 119, 148, 199, 166, 36, 95, 185, 13, 2, 169, 86, 50, 149, 100, 173, 185, 5, 193, 124,
159, 122, 140, 199, 241, 111, 76, 12, 252, 122, 188, 62, 174, 246, 68, 220, 76, 238, 200, 177, 95, 103, 45, 13, 182, 52, 199, 74, 194, 239,
119, 124, 158, 18, 254, 250, 150, 75, 83, 146, 244, 91, 208, 183, 90, 23, 177, 172, 34, 213, 26, 141, 174, 49, 96, 58, 35, 37, 88, 184,
210, 128, 145, 244, 7, 153, 197, 41, 110, 177, 129, 165, 34, 30, 57, 89, 127, 104, 105, 110, 234, 53, 131, 239, 64, 41, 125, 139, 54, 235,
14, 204, 241, 163, 129, 47, 32, 81, 224, 83, 89, 114, 103, 248, 160, 118, 55, 236, 188, 106, 17
};
        static ShaderVariables[] variants;
        private static bool iscompiled = false;
        private static int GetIndex(ShaderFeatures features)
        {
            ShaderFeatures masked = (features & ((ShaderFeatures)(23)));
            if ((masked == ((ShaderFeatures)(1))))
            {
                return 1;
            }
            if ((masked == ((ShaderFeatures)(16))))
            {
                return 2;
            }
            if ((masked == ((ShaderFeatures)(17))))
            {
                return 3;
            }
            if ((masked == ((ShaderFeatures)(2))))
            {
                return 4;
            }
            if ((masked == ((ShaderFeatures)(3))))
            {
                return 5;
            }
            if ((masked == ((ShaderFeatures)(18))))
            {
                return 6;
            }
            if ((masked == ((ShaderFeatures)(19))))
            {
                return 7;
            }
            if ((masked == ((ShaderFeatures)(4))))
            {
                return 8;
            }
            if ((masked == ((ShaderFeatures)(5))))
            {
                return 9;
            }
            if ((masked == ((ShaderFeatures)(20))))
            {
                return 10;
            }
            if ((masked == ((ShaderFeatures)(21))))
            {
                return 11;
            }
            if ((masked == ((ShaderFeatures)(6))))
            {
                return 12;
            }
            if ((masked == ((ShaderFeatures)(7))))
            {
                return 13;
            }
            if ((masked == ((ShaderFeatures)(22))))
            {
                return 14;
            }
            if ((masked == ((ShaderFeatures)(23))))
            {
                return 15;
            }
            return 0;
        }
        public static ShaderVariables Get(ShaderFeatures features)
        {
            return variants[GetIndex(features)];
        }
        public static ShaderVariables Get()
        {
            return variants[0];
        }
        public static void Compile()
        {
            if (iscompiled)
            {
                return;
            }
            iscompiled = true;
            ShaderVariables.Log("Compiling Basic_PositionNormalColorTexture");
            string vertsrc;
            string fragsrc;
            vertsrc = ShCompHelper.FromArray(vertex_bytes);
            fragsrc = ShCompHelper.FromArray(fragment_bytes);
            variants = new ShaderVariables[16];
            variants[0] = ShaderVariables.Compile(vertsrc, fragsrc, "");
            variants[1] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#line 1\n");
            variants[2] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define VERTEX_LIGHTING\n#line 1\n");
            variants[3] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define VERTEX_LIGHTING\n#line 1\n");
            variants[4] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ET_ENABLED\n#line 1\n");
            variants[5] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define ET_ENABLED\n#line 1\n");
            variants[6] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define VERTEX_LIGHTING\n#define ET_ENABLED\n#line 1\n");
            variants[7] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define VERTEX_LIGHTING\n#define ET_ENABLED\n#line 1\n");
            variants[8] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define FADE_ENABLED\n#line 1\n");
            variants[9] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define FADE_ENABLED\n#line 1\n");
            variants[10] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define VERTEX_LIGHTING\n#define FADE_ENABLED\n#line 1\n");
            variants[11] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define VERTEX_LIGHTING\n#define FADE_ENABLED\n#line 1\n");
            variants[12] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ET_ENABLED\n#define FADE_ENABLED\n#line 1\n");
            variants[13] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define ET_ENABLED\n#define FADE_ENABLED\n#line 1\n");
            variants[14] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define VERTEX_LIGHTING\n#define ET_ENABLED\n#define FADE_ENABLED\n#line 1\n");
            variants[15] = ShaderVariables.Compile(vertsrc, fragsrc, "\n#define ALPHATEST_ENABLED\n#define VERTEX_LIGHTING\n#define ET_ENABLED\n#define FAD" +
                    "E_ENABLED\n#line 1\n");
        }
    }
}

VolumetricSpotlight.shadergraph is the original from https://github.com/Josephy5/FakeVolumetricLight-Unity
(main branch, "Fake Volumetric Light Shader" by Josephy5; the repository has no LICENSE file, credits MrTriPie's
LWRP volumetric light tutorial). It replaces the compiled Metal-only shader asset ripped from the shipped build,
keeping that asset's GUID so the ripped materials (VolumetricSpotlight.mat, MAT_VolumetricLight.mat) and the
VolumetricSpotLight_Cone prefab still reference it. The other files in this folder (mesh, noise texture,
materials, prefabs) are the ripped originals from the shipped build.

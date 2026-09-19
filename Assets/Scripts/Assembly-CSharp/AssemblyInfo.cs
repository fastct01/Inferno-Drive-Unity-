using System.Runtime.CompilerServices;

// The EditMode tests in Assets/Editor/Tests (Assembly-CSharp-Editor) exercise the post-source rule
// classes (ShadowRunPrototype.InfernoDrivingRules / InfernoLobby / InfernoDriving), which are internal.
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

using System;
namespace SatiRogue.lib.go_dot_net.src.provider;

/// <summary>
/// Represents a dependency on a value provided by a node higher in the
/// current scene tree.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DependencyAttribute : Attribute { }
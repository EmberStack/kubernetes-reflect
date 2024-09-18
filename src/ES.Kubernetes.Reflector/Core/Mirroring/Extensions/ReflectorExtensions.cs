﻿using ES.Kubernetes.Reflector.Core.Extensions;
using ES.Kubernetes.Reflector.Core.Mirroring.Constants;
using ES.Kubernetes.Reflector.Core.Resources;
using k8s;
using k8s.Models;

namespace ES.Kubernetes.Reflector.Core.Mirroring.Extensions;

public static class ReflectorExtensions
{
    public static ReflectorProperties GetReflectionProperties(this IKubernetesObject<V1ObjectMeta> resource)
    {
        return resource.EnsureMetadata().GetReflectionProperties();
    }

    public static ReflectorProperties GetReflectionProperties(this V1ObjectMeta metadata)
    {
        return new ReflectorProperties
        {
            Version = metadata.ResourceVersion,

            Allowed = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.Allowed, out bool allowed) && allowed,

            AllowedNamespaces = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.AllowedNamespaces, out string? allowedNamespaces)
                ? allowedNamespaces ?? string.Empty
                : string.Empty,

            AutoEnabled = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.AutoEnabled, out bool autoEnabled) && autoEnabled,

            AutoNamespaces = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.AutoNamespaces, out string? autoNamespaces)
                ? autoNamespaces ?? string.Empty
                : string.Empty,

            Labels = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.Labels, out bool labels) && labels,

            LabelsIncluded = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.LabelsIncluded, out string? labelsIncluded)
                ? labelsIncluded ?? string.Empty
                : string.Empty,

            Reflects = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.Reflects, out string? metaReflects)
                ? string.IsNullOrWhiteSpace(metaReflects) ? KubeRef.Empty :
                KubeRef.TryParse(metaReflects, out var metaReflectsRef) ? metaReflectsRef.Namespace == string.Empty
                    ? new KubeRef(metadata.NamespaceProperty, metaReflectsRef.Name)
                    : metaReflectsRef : KubeRef.Empty
                : KubeRef.Empty,

            IsAutoReflection = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.MetaAutoReflects, out bool metaAutoReflects) && metaAutoReflects,

            ReflectedVersion = metadata.SafeAnnotations()
                .TryGet(Annotations.Reflection.MetaReflectedVersion, out string? reflectedVersion)
                ? string.IsNullOrWhiteSpace(reflectedVersion) ? string.Empty : reflectedVersion
                : string.Empty
        };
    }
}
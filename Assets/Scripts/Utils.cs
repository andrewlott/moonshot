using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public static class Utils {

    private static Vector3 _viewSize = Vector3.zero;
    public static Vector3 ViewSize() {
        if (_viewSize == Vector3.zero) {
            _viewSize = Camera.main.ViewportToWorldPoint(Vector3.one) - Camera.main.ViewportToWorldPoint(Vector3.zero);
        }
        return _viewSize;
    }

    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match

        List<Type> derivedTypes = new List<Type>();
        Type derived = type.BaseType;
        while (derived != null) {
            if (derived == typeof(MonoBehaviour)) {
                break;
            }
            derivedTypes.Add(derived);
            derived = derived.BaseType;
        }

        IEnumerable<PropertyInfo> pinfos = type.GetProperties(bindingFlags);

        foreach (Type derivedType in derivedTypes) {
            pinfos = pinfos.Concat(derivedType.GetProperties(bindingFlags));
        }

        pinfos = from property in pinfos
                 where !(type == typeof(Rigidbody) && property.Name == "inertiaTensor") // Special case for Rigidbodies inertiaTensor which isn't catched for some reason.
                 where !(type == typeof(CircleCollider2D) && property.Name == "usedByComposite") // Special case for CircleCollider2D.usedByComposite assign attempt for 'Planet(Clone)' is not valid as collider is not capable of being composited.
                 where !property.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                 select property;
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                if (pinfos.Any(e => e.Name == $"shared{char.ToUpper(pinfo.Name[0])}{pinfo.Name.Substring(1)}")) {
                    continue;
                }
                try {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }

        IEnumerable<FieldInfo> finfos = type.GetFields(bindingFlags);

        foreach (var finfo in finfos) {

            foreach (Type derivedType in derivedTypes) {
                if (finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}")) {
                    continue;
                }
                finfos = finfos.Concat(derivedType.GetFields(bindingFlags));
            }
        }

        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }

        finfos = from field in finfos
                 where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                 select field;
        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }

        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
        return go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd) as T;
    }
}

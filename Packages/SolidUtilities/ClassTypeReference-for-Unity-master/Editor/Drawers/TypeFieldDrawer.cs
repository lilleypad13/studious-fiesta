﻿namespace TypeReferences.Editor.Drawers
{
    using System;
    using SolidUtilities.Editor.Helpers;
    using TypeReferences;
    using UnityEditor;
    using UnityEngine;
    using Util;
    using TypeCache = Util.TypeCache;

    /// <summary>
    /// Draws a <see cref="TypeReference"/> field and handles control over the drop-down list.
    /// </summary>
    internal class TypeFieldDrawer
    {
        private const string MissingSuffix = " {Missing}";
        private static readonly int ControlHint = typeof(TypeReferencePropertyDrawer).GetHashCode();

        private readonly SerializedTypeReference _serializedTypeRef;
        private readonly TypeDropdownDrawer _dropdownDrawer;
        private readonly bool _showShortName;
        private readonly bool _useBuiltInNames;

        private Rect _position;
        private bool _triggerDropDown;

        public TypeFieldDrawer(
            SerializedTypeReference serializedTypeRef,
            Rect position,
            TypeDropdownDrawer dropdownDrawer,
            bool showShortName,
            bool useBuiltInNames)
        {
            _serializedTypeRef = serializedTypeRef;
            _position = position;
            _dropdownDrawer = dropdownDrawer;
            _showShortName = showShortName;
            _useBuiltInNames = useBuiltInNames;
        }

        public void Draw()
        {
            EditorDrawHelper.WhileShowingMixedValue(
                _serializedTypeRef.TypeNameHasMultipleDifferentValues,
                DrawTypeSelectionControl);
        }

        private void DrawTypeSelectionControl()
        {
            int controlID = GUIUtility.GetControlID(ControlHint, FocusType.Keyboard, _position);
            _triggerDropDown = false;
            ReactToCurrentEvent(controlID);

            if ( ! _triggerDropDown)
                return;

            _dropdownDrawer.Draw(OnTypeSelected);
        }

        private void ReactToCurrentEvent(int controlID)
        {
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    OnMouseDown(controlID);
                    break;

                case EventType.KeyDown:
                    OnKeyDown(controlID);
                    break;

                case EventType.Repaint:
                    DrawFieldContent(controlID);
                    break;
            }
        }

        private void OnMouseDown(int controlID)
        {
            bool mouseFocusedOnElement = GUI.enabled && _position.Contains(Event.current.mousePosition);
            if (! mouseFocusedOnElement)
                return;

            GUIUtility.keyboardControl = controlID;
            _triggerDropDown = true;
            Event.current.Use();
        }

        private void OnKeyDown(int controlID)
        {
            bool keyboardFocusIsOnElement = GUI.enabled && GUIUtility.keyboardControl == controlID;

            bool necessaryKeyIsDown =
                Event.current.keyCode == KeyCode.Return
                || Event.current.keyCode == KeyCode.Space;

            if (keyboardFocusIsOnElement && necessaryKeyIsDown)
            {
                _triggerDropDown = true;
                Event.current.Use();
            }
        }

        private void DrawFieldContent(int controlID)
        {
            var typeParts = _serializedTypeRef.TypeNameAndAssembly.Split(',');
            string fullTypeName = typeParts[0].Trim();
            var fieldContent = new GUIContent(GetTypeToShow(fullTypeName));
            EditorStyles.popup.Draw(_position, fieldContent, controlID);
        }

        private string GetTypeToShow(string typeName)
        {
            if (_useBuiltInNames && TypeNameFormatter.TryReplaceWithBuiltInName(ref typeName, true))
                return typeName;

            if (_showShortName)
                typeName = TypeNameFormatter.GetShortName(typeName);

            if (typeName == string.Empty)
                return TypeReference.NoneElement;

            if (TypeCache.GetType(_serializedTypeRef.TypeNameAndAssembly) == null)
            {
                _serializedTypeRef.TryUpdatingTypeUsingGUID();

                if (TypeCache.GetType(_serializedTypeRef.TypeNameAndAssembly) == null)
                    return typeName + MissingSuffix;
            }

            return typeName;
        }

        private void OnTypeSelected(Type selectedType)
        {
            string selectedTypeNameAndAssembly = TypeReference.GetTypeNameAndAssembly(selectedType);

            if (_serializedTypeRef.TypeNameAndAssembly == selectedTypeNameAndAssembly)
                return;

            _serializedTypeRef.TypeNameAndAssembly = selectedTypeNameAndAssembly;
            GUI.changed = true;
        }
    }
}
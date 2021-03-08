﻿using System.Collections.Generic;
using System.Xml.Serialization;
using AdofaiTweaks.Core;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.KeyLimiter
{
    public class KeyLimiterSettings : TweakSettings
    {
        public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();
        public bool ShowKeyViewer { get; set; }
        public float KeyViewerSize { get; set; } = 100f;
        public float KeyViewerXPos { get; set; } = 0.89f;
        public float KeyViewerYPos { get; set; } = 0.03f;

        private Color _pressedOutlineColor;
        public Color PressedOutlineColor {
            get => _pressedOutlineColor;
            set {
                _pressedOutlineColor = value;
                PressedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedOutlineColor;
        public Color ReleasedOutlineColor {
            get => _releasedOutlineColor;
            set {
                _releasedOutlineColor = value;
                ReleasedOutlineColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedBackgroundColor;
        public Color PressedBackgroundColor {
            get => _pressedBackgroundColor;
            set {
                _pressedBackgroundColor = value;
                PressedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedBackgroundColor;
        public Color ReleasedBackgroundColor {
            get => _releasedBackgroundColor;
            set {
                _releasedBackgroundColor = value;
                ReleasedBackgroundColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _pressedTextColor;
        public Color PressedTextColor {
            get => _pressedTextColor;
            set {
                _pressedTextColor = value;
                PressedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        private Color _releasedTextColor;
        public Color ReleasedTextColor {
            get => _releasedTextColor;
            set {
                _releasedTextColor = value;
                ReleasedTextColorHex = ColorUtility.ToHtmlStringRGBA(value);
            }
        }

        [XmlIgnore]
        public bool IsListening { get; set; }

        [XmlIgnore]
        public string PressedOutlineColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedOutlineColorHex { get; set; }

        [XmlIgnore]
        public string PressedBackgroundColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedBackgroundColorHex { get; set; }

        [XmlIgnore]
        public string PressedTextColorHex { get; set; }

        [XmlIgnore]
        public string ReleasedTextColorHex { get; set; }

        public KeyLimiterSettings() {
            PressedOutlineColor = Color.white;
            ReleasedOutlineColor = Color.white;
            PressedBackgroundColor = Color.white;
            ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
            PressedTextColor = Color.black;
            ReleasedTextColor = Color.white;
        }
    }
}

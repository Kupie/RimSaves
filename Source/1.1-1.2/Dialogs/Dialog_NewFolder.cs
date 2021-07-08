﻿using System;
using Verse;
using RimWorld;
using UnityEngine;

namespace aRandomKiwi.ARS
{
    public class Dialog_NewFolder : Window
    {
        protected string curName;

        private bool focusedFolderNameField;
        private Action onCloseCb;

        protected virtual int MaxNameLength
        {
            get
            {
                return 28;
            }
        }

        public override  Vector2 InitialSize
        {
            get
            {
                return new Vector2(280f, 175f);
            }
        }

        public Dialog_NewFolder(Action eventOnClose)
        {
            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;

            onCloseCb = eventOnClose;
        }

        protected virtual AcceptanceReport NameIsValid(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }
            return true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            GUI.SetNextControlName("FolderNameField");
            string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), this.curName);
            if (text.Length < this.MaxNameLength)
            {
                this.curName = text;
            }
            if (!this.focusedFolderNameField)
            {
                UI.FocusControl("FolderNameField", this);
                this.focusedFolderNameField = true;
            }
            if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, false, true) || flag)
            {
                AcceptanceReport acceptanceReport = this.NameIsValid(this.curName);
                if (!acceptanceReport.Accepted)
                {
                    if (acceptanceReport.Reason.NullOrEmpty())
                    {
                        Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    }
                    else
                    {
                        Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                    }
                }
                else
                {
                    
                    Settings.folders.Add(this.curName);
                    Settings.curFolder = this.curName;
                    Utils.curModRef.WriteSettings();

                    if (this.onCloseCb != null)
                        this.onCloseCb();

                    Find.WindowStack.TryRemove(this, true);
                }
            }
        }
    }
}
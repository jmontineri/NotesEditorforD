﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NotesEditerforD
{
    public class ShortNote
    {
        public MusicScore2 musicscore;
        private Point position, startPosition, endPosition;
        private string noteStyle, airDirection;
        private ShortNote childNote, parentNote;
        private bool isChildNote, isParentNote;
        private int longNoteNumber, noteSize;
        private Point[] destPoints;//{ul, ur, ll}
        //*
        private Bitmap noteImage;
        //*/

        public ShortNote(MusicScore2 _musicscore, Point _position, Point _startPosition, Point _endPosition, int _noteSize, string _noteStyle, string _airDirection, int _longNoteNumber)
        {
            musicscore = _musicscore;
            position = _position;
            startPosition = _startPosition;
            endPosition = _endPosition;
            noteSize = _noteSize;//1-16
            noteStyle = _noteStyle;
            airDirection = _airDirection;
            longNoteNumber = _longNoteNumber;
            //
            childNote = null;
            parentNote = null;
            isChildNote = false;
            isParentNote = false;
            destPoints = new Point[3];

            noteImage = setNoteImage();
        }

        ///*
        public Bitmap NoteImage
        {
            get { return this.noteImage; }
        }
        //*/

        public ShortNote ChildNote
        {
            get { return this.childNote; }
            set { this.childNote = value; }
        }

        public ShortNote ParentNote
        {
            get { return this.parentNote; }
            set { this.parentNote = value; }
        }

        public bool IsChildNote
        {
            get { return this.isChildNote; }
            set { this.isChildNote = value; }
        }

        public bool IsParentNote
        {
            get { return this.isParentNote; }
            set { this.isParentNote = value; }
        }

        public Point[] DestPoints
        {
            get { return this.setDestPoints("MusicScore"); }
        }

        public string NoteStyle
        {
            get { return this.noteStyle; }
            set { this.noteStyle = value; }
        }

        public int NoteSize
        {
            get { return this.noteSize; }
            set { this.noteSize = value; }
        }

        public Point NotePosition
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Point StartPosition
        {
            get { return this.startPosition; }
            set { this.startPosition = value; }
        }

        public Point EndPosition
        {
            get { return this.endPosition; }
            set { this.endPosition = value; }
        }

        public string AirDirection
        {
            get { return this.airDirection; }
            set { this.airDirection = value; }
        }

        public int LongNoteNumber
        {
            get { return this.longNoteNumber; }
            set { this.longNoteNumber = value; }
        }

        public Bitmap setNoteImage()
        {
            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            cm.Matrix00 = 1; cm.Matrix11 = 1; cm.Matrix22 = 1; cm.Matrix33 = 1; cm.Matrix44 = 1;
            Bitmap canvas;
            switch (noteStyle)
            {
                case "HoldLine":
                    canvas = new Bitmap(noteSize * 10, startPosition.Y - endPosition.Y == 0 ? 1 : startPosition.Y - endPosition.Y);
                    cm.Matrix33 = 0.9f;
                    break;
                case "SlideLine":
                    canvas = new Bitmap(Math.Abs(startPosition.X - endPosition.X) + noteSize * 10, startPosition.Y - endPosition.Y == 0 ? 1 : startPosition.Y - endPosition.Y);
                    cm.Matrix33 = 0.9f;
                    break;
                case "AirLine":
                    canvas = new Bitmap(noteSize * 10, startPosition.Y - endPosition.Y == 0 ? 1 : startPosition.Y - endPosition.Y);
                    cm.Matrix33 = 0.9f;
                    break;
                case "AirUp":
                    canvas = new Bitmap(noteSize * 10, 30);
                    break;
                case "AirDown":
                    canvas = new Bitmap(noteSize * 10, 30);
                    break;
                default:
                    canvas = new Bitmap(noteSize * 10, 5);
                    break;
            }
            System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(cm);
            Graphics g = Graphics.FromImage(canvas);
            Bitmap _noteImage = setNoteImage(noteStyle);
            _noteImage.MakeTransparent(Color.Black);
            destPoints = setDestPoints("ShortNote");
            //g.DrawImage(_noteImage ,destPoints, new Rectangle(new Point(0, 0), canvas.Size), GraphicsUnit.Pixel, ia);
            g.DrawImage(_noteImage, destPoints);
            g.Dispose();
            _noteImage.Dispose();
            return canvas;
        }
        //*
        private Bitmap setNoteImage(string _noteStyle)
        {
            Bitmap noteImage;
            switch (_noteStyle)
            {
                case "Tap":
                    noteImage = Properties.Resources.Tap;
                    break;
                case "ExTap":
                    noteImage = Properties.Resources.ExTap;
                    break;
                case "Flick":
                    noteImage = Properties.Resources.Flick;
                    break;
                case "HellTap":
                    noteImage = Properties.Resources.HellTap;
                    break;
                case "Hold":
                    noteImage = Properties.Resources.Hold;
                    break;
                case "HoldLine":
                    noteImage = Properties.Resources.HoldLine;
                    break;
                case "HoldEnd":
                    noteImage = Properties.Resources.HoldEnd;
                    break;
                case "Slide":
                    noteImage = Properties.Resources.Slide;
                    break;
                case "SlideLine":
                    noteImage = Properties.Resources.SlideLine;
                    break;
                case "SlideTap":
                    noteImage = Properties.Resources.SlideTap;
                    break;
                case "AirAction":
                    noteImage = Properties.Resources.AirAction;
                    break;
                case "AirLine":
                    noteImage = Properties.Resources.AirLine;
                    break;
                case "AirUp":
                    switch (airDirection)
                    {
                        case "Left":
                            noteImage = Properties.Resources.AirUpL;
                            break;
                        case "Center":
                            noteImage = Properties.Resources.AirUpC;
                            break;
                        case "Right":
                            noteImage = Properties.Resources.AirUpR;
                            break;
                        default:
                            noteImage = Properties.Resources.AirUpC;
                            break;
                    }
                    break;
                case "AirDown":
                    switch (airDirection)
                    {
                        case "Left":
                            noteImage = Properties.Resources.AirDownL;
                            break;
                        case "Center":
                            noteImage = Properties.Resources.AirDownC;
                            break;
                        case "Right":
                            noteImage = Properties.Resources.AirDownR;
                            break;
                        default:
                            noteImage = Properties.Resources.AirDownC;
                            break;
                    }
                    break;
                default:
                    noteImage = Properties.Resources.Tap;
                    break;
            }
            return noteImage;
        }
        //*/
        private Point[] setDestPoints(string state)
        {
            Point[] _destPoints = new Point[3];
            switch (noteStyle)
            {
                case "HoldEnd":
                    if(state == "ShortNote") _destPoints = new Point[3] { new Point(0, 0), new Point(10 * noteSize, 0), new Point(0, 5) };
                    else _destPoints = new Point[3] { new Point(startPosition.X, position.Y), new Point(startPosition.X + 10 * noteSize, position.Y), new Point(startPosition.X, position.Y + 5) };
                    break;
                case "HoldLine":
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(2, 0), new Point(10 * noteSize - 2, 0), new Point(2, startPosition.Y - endPosition.Y) };
                    else _destPoints = new Point[3] { new Point(startPosition.X + 2, endPosition.Y), new Point(startPosition.X + 10 * noteSize - 2, endPosition.Y), new Point(startPosition.X + 2, startPosition.Y) };
                    break;
                case "SlideLine":
                    if (state == "ShortNote")
                    {
                        if(startPosition.X > endPosition.X) _destPoints = new Point[3] { new Point(2, 0), new Point(10 * noteSize - 2, 0), new Point(startPosition.X - endPosition.X + 2, startPosition.Y - endPosition.Y) };
                        else _destPoints = new Point[3] { new Point(endPosition.X - startPosition.X + 2, 0), new Point(endPosition.X - startPosition.X + 10 * noteSize - 2, 0), new Point(2, startPosition.Y - endPosition.Y) };
                    }
                    else _destPoints = new Point[3] { new Point(endPosition.X + 2, endPosition.Y), new Point(endPosition.X + 10 * noteSize - 2, endPosition.Y), new Point(startPosition.X + 2, startPosition.Y) };
                    break;
                case "AirLine":
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(2, 0), new Point(10 * noteSize - 2, 0), new Point(2, startPosition.Y - endPosition.Y) };
                    else _destPoints = new Point[3] { new Point(startPosition.X + 5 * noteSize - 3, endPosition.Y), new Point(startPosition.X + 5 * noteSize + 3, endPosition.Y), new Point(startPosition.X + 5 * noteSize - 3, startPosition.Y) };
                    break;
                case "AirAction":
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(2, 0), new Point(10 * noteSize - 2, 0), new Point(2, 3) };
                    else _destPoints = new Point[3] { new Point(startPosition.X + 2, position.Y), new Point(startPosition.X + 10 * noteSize - 2, position.Y), new Point(startPosition.X + 2, position.Y + 3) };
                    break;
                case "AirUp":
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(0, 0), new Point(10 * noteSize, 0), new Point(0, 30) };
                    else _destPoints = new Point[3] { new Point(position.X, position.Y - 32), new Point(position.X + 10 * noteSize, position.Y - 32), new Point(position.X, position.Y + 30 - 32) };
                    break;
                case "AirDown":
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(0, 0), new Point(10 * noteSize, 0), new Point(0, 30) };
                    else _destPoints = new Point[3] { new Point(position.X, position.Y - 32), new Point(position.X + 10 * noteSize, position.Y - 32), new Point(position.X, position.Y + 30 - 32) };
                    break;
                default:
                    if (state == "ShortNote") _destPoints = new Point[3] { new Point(0, 0), new Point(10 * noteSize, 0), new Point(0, 5) };
                    else _destPoints = new Point[3] { position, new Point(position.X + 10 * noteSize, position.Y), new Point(position.X, position.Y + 5) };
                    break;
            }
            return _destPoints;
        }
    }
}
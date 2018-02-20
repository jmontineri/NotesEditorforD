﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace NotesEditerforD
{
    public class MusicScore : PictureBox
    {
        private static int selectedBeat, selectedGrid, selectedNoteSize, tmpLongNoteNumber;
        private static int topMargin = 5, bottomMargin = 5, leftMargin = 20, rightMargin = 30;
        private int index;
        private static string selectedNoteStyle, selectedEditStatus, selectedAirDirection;
        private static decimal selectedBPM, selectedSpeed;
        private Point startPosition, endPosition;
        private bool addSlideRelayFlag, previewVisible;
        public ScoreRoot sRoot;
        public List<ShortNote> shortNotes = new List<ShortNote>();
        public List<ShortNote> dummyNotes = new List<ShortNote>();
        public List<ShortNote> specialNotes = new List<ShortNote>();
        private Bitmap storeImage;
        private ShortNote previewNote, previewLongNote, startNote, selectedNote, selectedNote_prev, selectedNote_next;
        private MusicScore prevScore, nextScore;
        private MouseButtons eyedropperMouseButton = MouseButtons.Right;
        
        public MusicScore()
        {
            selectedBeat = 8;
            selectedGrid = 8;
            selectedNoteSize = 4;
            selectedEditStatus = "Add";
            selectedAirDirection = "Center";
            Location = new Point(0, 0);
            Margin = new Padding(10, 7, 10, 7);
            Size = new Size(160 + leftMargin + rightMargin, 768 + topMargin + bottomMargin);
            BackgroundImage = Properties.Resources.MusicScore;
            storeImage = Properties.Resources.MusicScore;
            previewNote = null;
            previewLongNote = null;
            prevScore = null;
            nextScore = null;
            addSlideRelayFlag = false;
            previewVisible = true;

            this.MouseDown += new MouseEventHandler(this_MouseDown);
            this.MouseMove += new MouseEventHandler(this_MouseMove);
            this.MouseUp += new MouseEventHandler(this_MouseUp);
            this.MouseEnter += new EventHandler(this_MouseEnter);
            this.MouseLeave += new EventHandler(this_MouseLeave);
        }

        public MusicScore PrevScore
        {
            get { return this.prevScore; }
            set { this.prevScore = value; }
        }

        public MusicScore NextScore
        {
            get { return this.nextScore; }
            set { this.nextScore = value; }
        }

        public static int SelectedBeat
        {
            get { return selectedBeat; }
            set { selectedBeat = value; }
        }

        public static int SelectedGrid
        {
            get { return selectedGrid; }
            set { selectedGrid = value; }
        }

        public static int SelectedNoteSize//1-16
        {
            get { return selectedNoteSize; }
            set { selectedNoteSize = value; }
        }

        public static string SelectedNoteStyle
        {
            get { return selectedNoteStyle; }
            set { selectedNoteStyle = value; }
        }

        public static string SelectedAirDirection
        {
            get { return selectedAirDirection; }
            set { selectedAirDirection = value; }
        }

        public static string SelectedEditStatus
        {
            get { return selectedEditStatus; }
            set { selectedEditStatus = value; }
        }

        public static decimal SelectedBPM
        {
            get { return selectedBPM; }
            set { selectedBPM = value; }
        }

        public static decimal SelectedSpeed
        {
            get { return selectedSpeed; }
            set { selectedSpeed = value; }
        }

        public static int TopMargin
        {
            get { return topMargin; }
        }

        public static int BottomMargin
        {
            get { return bottomMargin; }
        }

        public static int LeftMargin
        {
            get { return leftMargin; }
        }

        public static int RightMargin
        {
            get { return rightMargin; }
        }

        public void setNote(string[] _noteData, string dymsVersion)//dymsVer変更時に必ず編集
        {
            Point notePosition;
            int noteSize, longNoteNumber;
            string noteStyle, airDirection;
            ShortNote shortNote;
            if (dymsVersion == "0.3" || dymsVersion == "0.4" || dymsVersion == "0.5")
            {
                notePosition = new Point(int.Parse(_noteData[2]), int.Parse(_noteData[3]));
                startPosition = new Point(int.Parse(_noteData[4]), int.Parse(_noteData[5]));
                endPosition = new Point(int.Parse(_noteData[6]), int.Parse(_noteData[7]));
                noteSize = int.Parse(_noteData[1]);
                noteStyle = _noteData[0];
                airDirection = _noteData[8];
                longNoteNumber = int.Parse(_noteData[9]);
                if(dymsVersion == "0.5")
                {
                    shortNote = new ShortNote(this, notePosition, startPosition, endPosition, noteSize, noteStyle, airDirection, longNoteNumber);
                    shortNotes.Add(shortNote);
                    return;
                }
            }
            else if (dymsVersion != "0.1")
            {
                notePosition = new Point(int.Parse(_noteData[2]) + leftMargin - 5, int.Parse(_noteData[3]));
                startPosition = new Point(int.Parse(_noteData[4]) + leftMargin - 5, int.Parse(_noteData[5]));
                endPosition = new Point(int.Parse(_noteData[6]) + leftMargin - 5, int.Parse(_noteData[7]));
                noteSize = int.Parse(_noteData[1]);
                noteStyle = _noteData[0];
                airDirection = _noteData[8];
                longNoteNumber = int.Parse(_noteData[9]);
            }
            else
            {
                notePosition = new Point(int.Parse(_noteData[3]) + leftMargin - 5, int.Parse(_noteData[4]));
                startPosition = new Point(int.Parse(_noteData[5]) + leftMargin - 5, int.Parse(_noteData[6]));
                endPosition = new Point(int.Parse(_noteData[7]) + leftMargin - 5, int.Parse(_noteData[8]));
                noteSize = int.Parse(_noteData[1]) / 10;
                noteStyle = _noteData[0];
                airDirection = _noteData[9];
                longNoteNumber = int.Parse(_noteData[10]);
                if (noteStyle == "AirLine") noteSize = int.Parse(_noteData[11]);
                if (noteStyle == "AirAction") { noteSize += 1; notePosition.X -= 2; }
                if (noteStyle == "AirUp" || noteStyle == "AirDown") notePosition.Y += 32;
            }
            shortNote = new ShortNote(this, notePosition, startPosition, endPosition, noteSize, noteStyle, airDirection, longNoteNumber);
            addNote(shortNote);
            /*
            if(dymsVersion != "0.5" && shortNote.NoteStyle == "SlideLine")
            {
                foreach(ShortNote note in shortNotes)
                {
                    if(note.LongNoteNumber == shortNote.LongNoteNumber && note.NotePosition == shortNote.StartPosition)
                    {
                        if(note.NoteStyle == "SlideEnd")
                        {
                            note.NoteStyle = "SlideRelay";
                        }
                        else if(note.NoteStyle == "Slide" || note.NoteStyle == "SlideTap")
                        {
                            
                        }
                        
                        break;
                    }
                }
            }
            //*/
        }

        public void setSpecialNote(string[] _noteData)
        {
            Point notePosition;
            decimal specialValue;
            string noteStyle;
            notePosition = new Point(int.Parse(_noteData[1]), int.Parse(_noteData[2]));
            noteStyle = _noteData[0];
            specialValue = decimal.Parse(_noteData[3]);
            ShortNote shortNote = new ShortNote(this, notePosition, noteStyle, specialValue);
            specialNotes.Add(shortNote);
        }

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            if (selectedEditStatus == "Add")
            {
                bool deleteFlag = false;//ロングノーツ継ぎ足しの時trueにする
                if (e.Button == eyedropperMouseButton)//スポイト機能
                {
                    foreach (ShortNote note in shortNotes.Reverse<ShortNote>())
                    {
                        if (isMouseCollision(note.DestPoints, e.Location))
                        {
                            if (note.NoteStyle == "SlideLine" || note.NoteStyle == "SlideTap" || note.NoteStyle == "SlideRelay" || note.NoteStyle == "SlideEnd") selectedNoteStyle = "Slide";
                            else if (note.NoteStyle == "HoldLine" || note.NoteStyle == "HoldEnd") selectedNoteStyle = "Hold";
                            else if (note.NoteStyle == "AirAction" || note.NoteStyle == "AirBegin" || note.NoteStyle == "AirEnd") selectedNoteStyle = "AirLine";
                            else selectedNoteStyle = note.NoteStyle;
                            if (note.NoteStyle == "SlideCurve") selectedNoteSize = 4;
                            else selectedNoteSize = note.NoteSize;
                            selectedAirDirection = note.AirDirection;
                            break;
                        }
                    }
                    return;
                }
                ShortNote shortNote;
                startPosition = locationize(e.Location);
                if (selectedNoteStyle == "BPM")
                {
                    shortNote = new ShortNote(this, locationize(e.Location), selectedNoteStyle, selectedBPM);
                    if (shortNote.NotePosition.Y != 2 && ((772 - shortNote.NotePosition.Y) - 2) % 48 == 0) specialNotes.Add(shortNote);
                    update();
                    return;
                }
                if (selectedNoteStyle == "Speed")
                {
                    shortNote = new ShortNote(this, locationize(e.Location), selectedNoteStyle, selectedSpeed);
                    if (shortNote.NotePosition.Y != 2) specialNotes.Add(shortNote);
                    update();
                    return;
                }
                shortNote = new ShortNote(this, locationize(e.Location), startPosition, startPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, -1);
                //addNote(shortNote);
                startNote = shortNote;
                if (selectedNoteStyle == "Hold")
                {
                    shortNote.LongNoteNumber = sRoot.LongNoteNumber;
                    tmpLongNoteNumber = shortNote.LongNoteNumber;
                    foreach (ShortNote _note in shortNotes.Reverse<ShortNote>())
                    {
                        if (_note.NotePosition == locationize(e.Location) && _note.NoteSize == selectedNoteSize && _note.NoteStyle == "HoldEnd")//末尾に継ぎ足し
                        {
                            selectedNote = new ShortNote(this, _note.NotePosition, _note.StartPosition, _note.EndPosition, _note.NoteSize, _note.NoteStyle, _note.AirDirection, _note.LongNoteNumber);//MouseUp内で使う
                            tmpLongNoteNumber = _note.LongNoteNumber;
                            deleteFlag = true;//deleteNote(startNote);
                            deleteNote(_note);
                            break;
                        }
                    }
                    previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "HoldLine", selectedAirDirection, 0);
                }
                else if (selectedNoteStyle == "AirLine")
                {
                    startNote.NoteStyle = "AirBegin";
                    startNote.update();
                    shortNote.LongNoteNumber = sRoot.LongNoteNumber;
                    tmpLongNoteNumber = shortNote.LongNoteNumber;
                    foreach (ShortNote _note in shortNotes.Reverse<ShortNote>())
                    {
                        if (_note.NotePosition == locationize(e.Location) && _note.NoteSize == selectedNoteSize && _note.NoteStyle == "AirEnd")//末尾に継ぎ足し
                        {
                            deleteFlag = true;//deleteNote(startNote);
                            tmpLongNoteNumber = _note.LongNoteNumber;
                            _note.NoteStyle = "AirAction";
                            _note.update();
                            selectedNote = _note;//MouseUpで使う
                            if (_note.PrevNote != null)
                            {
                                _note.PrevNote.NoteStyle = "AirAction";
                                _note.PrevNote.update();
                                prevScore.update();
                            }
                            break;
                        }

                        if (_note.NoteStyle == "AirLine" && _note.NoteSize == selectedNoteSize && isMouseCollision(_note.DestPoints, e.Location))//間に節を追加
                        {
                            Point newPos = new Point(_note.EndPosition.X, locationize(e.Location).Y);
                            ShortNote airAction = new ShortNote(this, newPos, newPos, newPos, _note.NoteSize, "AirAction", "Center", _note.LongNoteNumber);
                            shortNotes.Insert(shortNotes.IndexOf(_note) + 1, airAction);
                            ShortNote next = _note;
                            ShortNote prev = new ShortNote(this, next.NotePosition, next.StartPosition, newPos, next.NoteSize, "AirLine", "Center", next.LongNoteNumber);
                            next.StartPosition = newPos;
                            shortNotes.Insert(shortNotes.IndexOf(_note), prev);
                            next.update();
                            addSlideRelayFlag = true;
                            break;
                        }
                    }
                    previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "AirLine", selectedAirDirection, 0);
                }
                else if (selectedNoteStyle == "Slide")
                {
                    shortNote.LongNoteNumber = sRoot.LongNoteNumber;
                    tmpLongNoteNumber = shortNote.LongNoteNumber;
                    for (int i = shortNotes.Count - 1; i >= 0; i--)
                    {
                        if (shortNotes[i].NotePosition == locationize(e.Location) && shortNotes[i].NoteSize == selectedNoteSize && shortNotes[i].NoteStyle == "SlideEnd")//末尾から継ぎ足し
                        {
                            tmpLongNoteNumber = shortNotes[i].LongNoteNumber;
                            deleteFlag = true;//deleteNote(startNote);
                            if (sRoot.SlideRelay) shortNotes[i].NoteStyle = "SlideTap";
                            else shortNotes[i].NoteStyle = "SlideRelay";
                            shortNotes[i].update();
                            if (shortNotes[i].PrevNote != null)
                            {
                                shortNotes[i].PrevNote.NoteStyle = shortNotes[i].NoteStyle;
                                shortNotes[i].PrevNote.update();
                                prevScore.update();
                            }
                            selectedNote = shortNotes[i];//MouseUp内で使う
                            break;
                        }

                        if (shortNotes[i].NoteStyle == "SlideLine" && shortNotes[i].NoteSize == selectedNoteSize && isMouseCollision(shortNotes[i].DestPoints, e.Location))//間に節を追加
                        {
                            if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift) break;
                            deleteFlag = true;//deleteNote(startNote);
                            previewVisible = false;
                            ShortNote next = shortNotes[i];
                            ShortNote prev = new ShortNote(this, next.NotePosition, next.StartPosition, locationize(e.Location), next.NoteSize, "SlideLine", "Center", next.LongNoteNumber);
                            next.StartPosition = locationize(e.Location);
                            //addNote(prev);
                            shortNotes.Insert(i, prev);
                            next.update();
                            ShortNote slide;
                            if (!sRoot.SlideRelay)
                            {
                                slide = new ShortNote(this, locationize(e.Location), locationize(e.Location), locationize(e.Location), next.NoteSize, "SlideRelay", "Center", next.LongNoteNumber);
                            }
                            else
                            {
                                slide = new ShortNote(this, locationize(e.Location), locationize(e.Location), locationize(e.Location), next.NoteSize, "SlideTap", "Center", next.LongNoteNumber);
                            }
                            //addNote(slide);
                            shortNotes.Insert(i + 1, slide);
                            selectedNote = slide;
                            selectedNote_next = next;
                            selectedNote_prev = prev;
                            //previewNote = null; previewLongNote = null;
                            addSlideRelayFlag = true;
                            break;
                        }
                    }
                    if (!addSlideRelayFlag) previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "SlideLine", selectedAirDirection, 0);
                }
                else if (selectedNoteStyle == "SlideCurve")
                {
                    deleteFlag = true;//deleteNote(shortNote);
                    foreach (ShortNote note in shortNotes.Reverse<ShortNote>())
                    {
                        if (note.NoteStyle == "SlideLine" && isMouseCollision(note.DestPoints, e.Location))
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), locationize(e.Location), locationize(e.Location), note.NoteSize, "SlideCurve", "Center", note.LongNoteNumber);
                            addNote(shortNote);
                            selectedNote = shortNote;
                            addSlideRelayFlag = true;
                            previewVisible = false;
                            break;
                        }
                    }
                }
                shortNote.setRelativePosition();
                if(!deleteFlag) addNote(shortNote);
                update();
            }
            else if (selectedEditStatus == "Delete")
            {
                bool flg = false;
                foreach (ShortNote _note in shortNotes.Reverse<ShortNote>())//普通のノーツ
                {
                    if (isMouseCollision(_note.DestPoints, e.Location))
                    {
                        //System.Diagnostics.Debug.WriteLine(_note.NoteStyle + " " + _note.NotePosition + " " + _note.EndPosition);
                        if (_note.NoteStyle == "HoldLine" || _note.NoteStyle == "SlideLine" || _note.NoteStyle == "AirLine") break;
                        else if (_note.NoteStyle == "SlideTap" || _note.NoteStyle == "SlideRelay")
                        {
                            ShortNote prev = null, next = null;
                            foreach (ShortNote _prev in shortNotes)
                            {
                                if (_prev.NoteStyle == "SlideLine" && _prev.LongNoteNumber == _note.LongNoteNumber && _prev.EndPosition == _note.NotePosition)
                                {
                                    prev = _prev;
                                    break;
                                }
                            }
                            foreach (ShortNote _next in shortNotes)
                            {
                                if (_next.NoteStyle == "SlideLine" && _next.LongNoteNumber == _note.LongNoteNumber && _next.StartPosition == _note.NotePosition)
                                {
                                    next = _next;
                                    break;
                                }
                            }
                            if (next != null)
                            {
                                prev.EndPosition = next.EndPosition;
                                prev.update();
                                deleteNote(next);
                                deleteNote(_note);
                            }
                        }
                        else if (_note.NoteStyle == "AirAction")
                        {
                            ShortNote prev = null, next = null;
                            foreach (ShortNote _prev in shortNotes)
                            {
                                if (_prev.NoteStyle == "AirLine" && _prev.LongNoteNumber == _note.LongNoteNumber && _prev.EndPosition == _note.NotePosition)
                                {
                                    prev = _prev;
                                    break;
                                }
                            }
                            foreach (ShortNote _next in shortNotes)
                            {
                                if (_next.NoteStyle == "AirLine" && _next.LongNoteNumber == _note.LongNoteNumber && _next.StartPosition == _note.NotePosition)
                                {
                                    next = _next;
                                    break;
                                }
                            }
                            if (next != null)
                            {
                                prev.EndPosition = next.EndPosition;
                                prev.update();
                                deleteNote(next);
                                deleteNote(_note);
                            }
                        }
                        else if (_note.NoteStyle == "Slide" || _note.NoteStyle == "Hold" || _note.NoteStyle == "AirBegin")
                        {
                            int number = _note.LongNoteNumber;
                            if (number == -1) { deleteNote(_note); return; }
                            string style;
                            if (_note.NoteStyle == "Slide") style = "SlideEnd";
                            else if (_note.NoteStyle == "Hold") style = "HoldEnd";
                            else style = "AirEnd";
                            bool isOver = false;
                            for (MusicScore score = this; score != null; score = score.nextScore)
                            {
                                foreach (ShortNote __note in score.shortNotes.ToArray())
                                {
                                    if (__note.LongNoteNumber == number && __note.NoteStyle == style) isOver = true;
                                    if (__note.LongNoteNumber == number) score.deleteNote(__note);
                                }
                                if (isOver) break; //{ MessageBox.Show("break!"); break; }
                            }
                            break;
                        }
                        else if (_note.NoteStyle == "SlideEnd" || _note.NoteStyle == "HoldEnd" || _note.NoteStyle == "AirEnd")
                        {
                            int number = _note.LongNoteNumber;
                            if (number == -1) { deleteNote(_note); return; }
                            string style;
                            if (_note.NoteStyle == "SlideEnd") style = "Slide";
                            else if (_note.NoteStyle == "HoldEnd") style = "Hold";
                            else style = "AirBegin";
                            bool isOver = false;
                            for (MusicScore score = this; score != null; score = score.prevScore)
                            {
                                if (score == null) break;
                                foreach (ShortNote __note in score.shortNotes.ToArray())
                                {
                                    if (__note.LongNoteNumber == number && __note.NoteStyle == style) isOver = true;
                                    if (__note.LongNoteNumber == number) score.deleteNote(__note);
                                }
                                if (isOver) break; //{ MessageBox.Show("Over!"); break; }
                            }
                            break;
                        }
                        deleteNote(_note);
                        flg = true;
                        break;
                    }
                }
                if (!flg)
                {
                    foreach (ShortNote _dummy in dummyNotes.Reverse<ShortNote>())//ダミーノーツ
                    {
                        if (isMouseCollision(_dummy.DestPoints, e.Location))
                        {
                            deleteNote(_dummy);
                            flg = true;
                            break;
                        }
                    }
                }
                if (!flg)
                {
                    foreach (ShortNote _special in specialNotes.Reverse<ShortNote>())//特殊ノーツ
                    {
                        if (isMouseCollision(_special.DestPoints, e.Location))
                        {
                            //deleteNote(specialNotes[i]);
                            specialNotes.Remove(_special);
                            break;
                        }
                    }
                }
            }
            else if (selectedEditStatus == "Edit")
            {
                foreach(ShortNote _note in shortNotes.Reverse<ShortNote>())
                {
                    if(_note.NoteStyle != "SlideLine" && _note.NoteStyle != "HoldLine" && _note.NoteStyle != "AirLine" && isMouseCollision(_note.DestPoints, e.Location))
                    {
                        selectedNote = _note; //MessageBox.Show("Hit");
                        foreach(ShortNote __note in shortNotes)
                        {
                            if(__note != _note && __note.LongNoteNumber == _note.LongNoteNumber && __note.LongNoteNumber != -1 && __note.EndPosition == _note.NotePosition && _note.NoteStyle != "SlideCurve")
                            {
                                selectedNote_prev = __note;
                                break;
                            }
                        }
                        foreach (ShortNote __note in shortNotes)
                        {
                            if (__note != _note && __note.LongNoteNumber == _note.LongNoteNumber && __note.LongNoteNumber != -1 && __note.StartPosition == _note.NotePosition && _note.NoteStyle != "SlideCurve")
                            {
                                selectedNote_next = __note;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            sRoot.setEdited(true);
        }

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedEditStatus == "Add" && e.Button == eyedropperMouseButton) return;//スポイト機能使用時
            if (selectedEditStatus == "Add" && !addSlideRelayFlag)
            {
                if (previewNote != null)
                {
                    //if (selectedNoteStyle == "AirLine") previewNote.NotePosition = new Point();
                    previewNote.NotePosition = locationize(e.Location);
                }
                if (previewLongNote != null)
                {
                    if (selectedNoteStyle == "Hold")
                    {
                        previewNote.NotePosition = locationize(new Point(startNote.NotePosition.X, e.Y - 1));
                    }
                    else if (selectedNoteStyle == "Slide")
                    {

                    }
                    else if (selectedNoteStyle == "AirLine")
                    {
                        //MessageBox.Show(startNote.NoteStyle);
                        previewNote.NotePosition = locationize(new Point(startNote.NotePosition.X, e.Y - 1));//locationize(new Point(previewLongNote.StartPosition.X, e.Y - 1));
                    }
                    if (previewLongNote.StartPosition.Y > e.Y)
                    {
                        Cursor.Current = Cursors.Arrow;
                        previewLongNote.EndPosition = locationize(e.Location);
                    }
                    else Cursor.Current = Cursors.No;
                }
                if (selectedEditStatus != "Add" && previewNote != null) previewNote = null;
                else if (selectedEditStatus == "Add" && previewNote == null)//ショートカットキーによるAddモードへの変更時の処理
                {
                    if (selectedNoteStyle == "AirLine")
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "AirAction", selectedAirDirection, 0);
                    }
                    else
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, 0);
                    }
                }
                else if (selectedEditStatus == "Add" && previewNote != null && previewNote.NoteStyle != selectedNoteStyle)//previewノーツにノーツスタイルを反映
                {
                    if (selectedNoteStyle == "AirLine")
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "AirAction", selectedAirDirection, 0);
                    }
                    else
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, 0);
                    }
                }
                else if (selectedEditStatus == "Add" && previewNote != null && previewNote.AirDirection != selectedAirDirection)//previewノーツにAirの向きを反映
                {
                    if (selectedNoteStyle == "AirLine")
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "AirAction", selectedAirDirection, 0);
                    }
                    else
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, 0);
                    }
                }
                else if (selectedEditStatus == "Add" && previewNote != null && previewNote.NoteSize != selectedNoteSize)//previewノーツにノーツサイズを反映
                {
                    if (selectedNoteStyle == "AirLine")
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "AirAction", selectedAirDirection, 0);
                    }
                    else
                    {
                        previewNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, 0);
                    }
                }
            }
            else if (selectedEditStatus == "Edit" || addSlideRelayFlag)
            {
                if (previewNote != null) previewNote = null;
                int threshold = 384 / selectedBeat;
                if (selectedNote != null)
                {
                    if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                    {
                        selectedNote.NotePosition = new Point(selectedNote.NotePosition.X, locationize(e.Location).Y);
                    }
                    else
                    {
                        selectedNote.NotePosition = locationize(e.Location, selectedNote.NoteSize);
                    }
                    fixBorder(selectedNote);
                }
                if (selectedNote_prev != null && selectedNote_next == null)
                {
                    if (selectedNote_prev.StartPosition.Y - selectedNote.NotePosition.Y >= threshold)
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            selectedNote_prev.EndPosition = new Point(selectedNote_prev.EndPosition.X, locationize(e.Location).Y);
                        }
                        else
                        {
                            selectedNote_prev.EndPosition = locationize(e.Location, selectedNote_prev.NoteSize);
                        }
                    }
                    else
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            Point binded = new Point(selectedNote.NotePosition.X, locationize(e.Location).Y);
                            selectedNote.NotePosition = new Point(locationize(binded, selectedNote.NoteSize).X, selectedNote_prev.StartPosition.Y - threshold);
                        }
                        else
                        {
                            selectedNote.NotePosition = new Point(locationize(e.Location, selectedNote.NoteSize).X, selectedNote_prev.StartPosition.Y - threshold);
                        }
                        selectedNote_prev.EndPosition = selectedNote.NotePosition;
                    }
                    selectedNote_prev.NotePosition = selectedNote.NotePosition;
                    selectedNote_prev.update();
                    fixBorder(selectedNote_prev);
                    //fixBorder(selectedNote);
                }
                else if (selectedNote_next != null && selectedNote_prev == null)
                {
                    if (selectedNote.NotePosition.Y - selectedNote_next.EndPosition.Y >= threshold)
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            selectedNote_next.StartPosition = new Point(selectedNote_next.StartPosition.X, locationize(e.Location).Y);
                        }
                        else
                        {
                            selectedNote_next.StartPosition = locationize(e.Location, selectedNote_next.NoteSize);
                        }
                        selectedNote_next.update();
                    }
                    else
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            Point binded = new Point(selectedNote.NotePosition.X, locationize(e.Location).Y);
                            selectedNote.NotePosition = new Point(locationize(binded, selectedNote.NoteSize).X, selectedNote_next.EndPosition.Y + threshold);
                        }
                        else
                        {
                            selectedNote.NotePosition = new Point(locationize(e.Location, selectedNote.NoteSize).X, selectedNote_next.EndPosition.Y + threshold);
                        }
                        selectedNote_next.StartPosition = selectedNote.NotePosition;
                        selectedNote_next.update();
                    }
                    fixBorder(selectedNote_next);
                    //fixBorder(selectedNote);
                }
                else if (selectedNote_prev != null && selectedNote_next != null)
                {
                    if (selectedNote_prev.StartPosition.Y - selectedNote.NotePosition.Y < threshold)
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            Point binded = new Point(selectedNote.NotePosition.X, locationize(e.Location).Y);
                            selectedNote.NotePosition = new Point(locationize(binded, selectedNote.NoteSize).X, selectedNote_prev.StartPosition.Y - threshold);
                        }
                        else
                        {
                            selectedNote.NotePosition = new Point(locationize(e.Location, selectedNote.NoteSize).X, selectedNote_prev.StartPosition.Y - threshold);
                        }
                        selectedNote_prev.EndPosition = selectedNote.NotePosition;
                        selectedNote_prev.update();
                        selectedNote_next.StartPosition = selectedNote.NotePosition;
                        selectedNote_next.update();
                    }
                    else if (selectedNote.NotePosition.Y - selectedNote_next.EndPosition.Y < threshold)
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            Point binded = new Point(selectedNote.NotePosition.X, locationize(e.Location).Y);
                            selectedNote.NotePosition = new Point(locationize(binded, selectedNote.NoteSize).X, selectedNote_next.EndPosition.Y + threshold);
                        }
                        else
                        {
                            selectedNote.NotePosition = new Point(locationize(e.Location, selectedNote.NoteSize).X, selectedNote_next.EndPosition.Y + threshold);
                        }
                        selectedNote_next.StartPosition = selectedNote.NotePosition;
                        selectedNote_next.update();
                        selectedNote_prev.EndPosition = selectedNote.NotePosition;
                        selectedNote_prev.update();
                    }
                    else
                    {
                        if (selectedNote.NoteStyle == "Hold" || selectedNote.NoteStyle == "HoldEnd" || selectedNote.NoteStyle == "AirAction" || selectedNote.NoteStyle == "AirBegin" || selectedNote.NoteStyle == "AirEnd")
                        {
                            selectedNote_prev.EndPosition = new Point(selectedNote_prev.EndPosition.X, locationize(e.Location).Y);
                            selectedNote_next.StartPosition = new Point(selectedNote_next.StartPosition.X, locationize(e.Location).Y);
                        }
                        else
                        {
                            selectedNote_prev.EndPosition = locationize(e.Location, selectedNote_prev.NoteSize);
                            selectedNote_next.StartPosition = locationize(e.Location, selectedNote_next.NoteSize);
                        }
                        selectedNote_prev.update();
                        selectedNote_next.update();
                    }
                    //fixBorder(selectedNote);
                    fixBorder(selectedNote_prev);
                    fixBorder(selectedNote_next);
                }
            }
            else if (selectedEditStatus == "Delete" && previewNote != null) previewNote = null;
            update();
        }

        private void this_MouseUp(object sender, MouseEventArgs e)
        {
            previewVisible = true;
            if (selectedEditStatus == "Add")
            {
                if (e.Button == eyedropperMouseButton) return;//スポイト機能使用時
                ShortNote shortNote;
                endPosition = locationize(e.Location);
                if (endPosition.Y < 2) endPosition.Y = 2;
                if (endPosition.X < leftMargin + 1) endPosition.X = leftMargin + 1;
                if (endPosition.X > 161 + leftMargin - selectedNoteSize * 10) endPosition.X = 161 + leftMargin - selectedNoteSize * 10;
                switch (selectedNoteStyle)
                {
                    case "Hold":
                        if (endPosition.Y < startPosition.Y)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "HoldLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, new Point(startPosition.X, endPosition.Y), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "HoldEnd", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            sRoot.LongNoteNumber++;
                        }
                        else if(selectedNote != null)
                        {
                            addNote(selectedNote);
                            selectedNote = null;
                            deleteNote(startNote);
                        }
                        else deleteNote(startNote);
                        break;
                    case "Slide":
                        if (endPosition.Y < startPosition.Y && !addSlideRelayFlag)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "SlideLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, endPosition, startPosition, endPosition, selectedNoteSize, "SlideEnd", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            sRoot.LongNoteNumber++;
                        }
                        else if (selectedNote != null && !addSlideRelayFlag)//Slideが新たに生成されなかったときの後始末
                        {
                            selectedNote.NoteStyle = "SlideEnd";
                            selectedNote.update();
                            if(selectedNote.PrevNote != null)
                            {
                                selectedNote.PrevNote.NoteStyle = "SlideEnd";
                                selectedNote.PrevNote.update();
                            }
                            selectedNote = null;
                            deleteNote(startNote);
                        }
                        else deleteNote(startNote);
                        break;
                    case "AirLine":
                        if (endPosition.Y < startPosition.Y && !addSlideRelayFlag)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "AirLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, new Point(startPosition.X, endPosition.Y), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "AirEnd", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            sRoot.LongNoteNumber++;
                        }
                        else if (selectedNote != null && !addSlideRelayFlag)//AirLineが新たに生成されなかったときの後始末
                        {
                            selectedNote.NoteStyle = "AirEnd";
                            selectedNote.update();
                            if (selectedNote.PrevNote != null)
                            {
                                selectedNote.PrevNote.NoteStyle = "AirEnd";
                                selectedNote.PrevNote.update();
                            }
                            selectedNote = null;
                        }
                        else deleteNote(startNote);
                        break;
                }
                previewLongNote = null;
                update();
            }
            else if (selectedEditStatus == "Edit")
            {
                
            }
            if(selectedNote != null) selectedNote = null;
            if(selectedNote_prev != null) selectedNote_prev = null;
            if(selectedNote_next != null) selectedNote_next = null;
            addSlideRelayFlag = false;
        }

        private void this_MouseEnter(object sender, EventArgs e)
        {
            if(selectedEditStatus == "Add")
            {
                if(SelectedNoteStyle == "AirLine")
                {
                    previewNote = new ShortNote(this, locationize(MousePosition), startPosition, endPosition, selectedNoteSize, "AirAction", selectedAirDirection, 0);
                }
                else
                {
                    previewNote = new ShortNote(this, locationize(MousePosition), startPosition, endPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, 0);
                }
                update();
            }
        }

        private void this_MouseLeave(object sender, EventArgs e)
        {
            if(SelectedEditStatus == "Add" && previewNote != null)
            {
                previewNote = null;
                update();
            }
        }

        private void addNote(ShortNote shortNote)//, string position)
        {
            fixBorder(shortNote);
            shortNotes.Add(shortNote);
        }

        private void fixBorder(ShortNote shortNote)
        {
            if (shortNote.NotePosition.Y == 770 && prevScore != null && shortNote.PrevNote == null && shortNote.NextNote == null)
            {
                ShortNote prevShortNote = new ShortNote(prevScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                prevShortNote.StartPosition = new Point(prevShortNote.StartPosition.X, prevShortNote.StartPosition.Y - 768);
                prevShortNote.EndPosition = new Point(prevShortNote.EndPosition.X, prevShortNote.EndPosition.Y - 768);
                prevShortNote.NotePosition = new Point(prevShortNote.NotePosition.X, prevShortNote.NotePosition.Y - 768);
                //prevShortNote.LongNoteNumber = shortNote.LongNoteNumber;
                prevShortNote.NextNote = shortNote;
                shortNote.PrevNote = prevShortNote;
                prevScore.shortNotes.Add(prevShortNote);
                prevScore.update();
            }
            else if (shortNote.NotePosition.Y == 2 && nextScore != null && shortNote.PrevNote == null && shortNote.NextNote == null)
            {
                ShortNote nextShortNote = new ShortNote(nextScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                nextShortNote.StartPosition = new Point(nextShortNote.StartPosition.X, nextShortNote.StartPosition.Y + 768);
                nextShortNote.EndPosition = new Point(nextShortNote.EndPosition.X, nextShortNote.EndPosition.Y + 768);
                nextShortNote.NotePosition = new Point(nextShortNote.NotePosition.X, nextShortNote.NotePosition.Y + 768);
                //nextShortNote.LongNoteNumber = shortNote.LongNoteNumber;
                nextShortNote.PrevNote = shortNote;
                shortNote.NextNote = nextShortNote;
                nextScore.shortNotes.Add(nextShortNote);
                nextScore.update();
            }
            //*
            if (shortNote.NotePosition.Y != 770 && shortNote.PrevNote != null && shortNote.NoteStyle != "SlideLine")
            {
                prevScore.shortNotes.Remove(shortNote.PrevNote);
                shortNote.PrevNote = null;
                prevScore.update();
            }
            else if(shortNote.NotePosition.Y != 2 && shortNote.NextNote != null && shortNote.NoteStyle != "SlideLine")
            {
                nextScore.shortNotes.Remove(shortNote.NextNote);
                shortNote.NextNote = null;
                nextScore.update();
            }
            //*/
            if (shortNote.PrevNote != null)
            {
                shortNote.PrevNote.NotePosition = new Point(shortNote.NotePosition.X, shortNote.PrevNote.NotePosition.Y);
                prevScore.update();
            }
            if (shortNote.NextNote != null)
            {
                shortNote.NextNote.NotePosition = new Point(shortNote.NotePosition.X, shortNote.NextNote.NotePosition.Y);
                nextScore.update();
            } 
            return;
        }

        private bool isMouseCollision(Point[] _destPoints, Point e)
        {
            Point upperLeft = _destPoints[0], upperRight = _destPoints[1], lowerLeft = _destPoints[2];
            Point vec_ULLL = new Point(lowerLeft.X - upperLeft.X, lowerLeft.Y - upperLeft.Y);//->ac
            Point vec_ULUR = new Point(upperRight.X - upperLeft.X, upperRight.Y - upperLeft.Y);//->ab
            Point vec_ULE = new Point(e.X - upperLeft.X, e.Y - upperLeft.Y);//->ap
            double s = (vec_ULE.X * vec_ULLL.Y - vec_ULLL.X * vec_ULE.Y) / (double)(vec_ULUR.X * vec_ULLL.Y - vec_ULLL.X * vec_ULUR.Y);
            double t = (vec_ULE.X * vec_ULUR.Y - vec_ULUR.X * vec_ULE.Y) / (double)(vec_ULLL.X * vec_ULUR.Y - vec_ULUR.X * vec_ULLL.Y);
            //MessageBox.Show("s = " + s + '\n' + "t = " + t);
            if (0 <= s && s <= 1 && 0 <= t && t <= 1) return true;
            return false;
        }

        public void update()
        {
            storeImage = Properties.Resources.MusicScore;
            Graphics g = Graphics.FromImage(storeImage);
            foreach (ShortNote _note in specialNotes)//BPMノーツ，Speedノーツを描画
            {
                if (_note.NoteStyle == "BPM")
                {
                    g.DrawImage(_note.NoteImage, _note.NotePosition);
                    g.DrawString(_note.SpecialValue.ToString(), new Font("ＭＳ ゴシック", 8, FontStyle.Bold), Brushes.Lime, new Rectangle(180, _note.NotePosition.Y - 5, 50, 15));//BPM
                }
                else
                {
                    g.DrawImage(_note.NoteImage, _note.NotePosition);
                    g.DrawString("x" + _note.SpecialValue.ToString(), new Font("ＭＳ ゴシック", 8, FontStyle.Bold), Brushes.Crimson, new Rectangle(180, _note.NotePosition.Y - 5, 50, 15));//Speed
                }
            }
            foreach (ShortNote _note in dummyNotes)//ダミーノーツを描画
            {
                if (_note.NoteStyle == "AirUp" || _note.NoteStyle == "AirDown") g.DrawImage(_note.NoteImage, new Point(_note.NotePosition.X, _note.NotePosition.Y - 32));
                else if (_note.NoteStyle == "HoldLine" || _note.NoteStyle == "AirLine") g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                else if (_note.NoteStyle == "SlideLine")
                {
                    if (_note.StartPosition.X >= _note.EndPosition.X) g.DrawImage(_note.NoteImage, _note.EndPosition);
                    else g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                }
                else g.DrawImage(_note.NoteImage, _note.NotePosition);
            }
            foreach (ShortNote _note in shortNotes)//普通のノーツを描画
            {
                if(_note.NoteStyle == "AirUp" || _note.NoteStyle == "AirDown") g.DrawImage(_note.NoteImage, new Point(_note.NotePosition.X, _note.NotePosition.Y - 32));
                else if(_note.NoteStyle == "HoldLine" || _note.NoteStyle == "AirLine") g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                else if(_note.NoteStyle == "SlideLine")
                {
                    if (_note.StartPosition.X >= _note.EndPosition.X) g.DrawImage(_note.NoteImage, _note.EndPosition);
                    else g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                }
                else g.DrawImage(_note.NoteImage, _note.NotePosition);
                if(_note.NoteStyle == "SlideCurve")//Bezier
                {
                    Point prev = new Point(-1, 9999), next = new Point(-1, -9999);
                    foreach(ShortNote note in shortNotes)
                    {
                        if(note.LongNoteNumber == _note.LongNoteNumber && (note.NoteStyle == "Slide" || note.NoteStyle == "SlideTap" || note.NoteStyle == "SlideRelay" || note.NoteStyle == "SlideEnd"))
                        {
                            if (prev.Y > note.NotePosition.Y && note.NotePosition.Y > _note.NotePosition.Y) prev = note.NotePosition;
                            else if (_note.NotePosition.Y > note.NotePosition.Y && note.NotePosition.Y > next.Y) next = note.NotePosition;
                        }
                    }
                    if(prev.X != -1 && next.X != -1)
                    {
                        int size = _note.NoteSize * 10;
                        float ratio = 11 / 16f;
                        Pen pen = new Pen(Color.SteelBlue, 1);
                        Point prevcur = new Point(prev.X + (int)((_note.NotePosition.X - prev.X) * ratio) + 2, prev.Y + (int)((_note.NotePosition.Y - prev.Y) * ratio));
                        Point curnext = new Point(next.X + (int)((_note.NotePosition.X - next.X) * ratio) + 2, next.Y + (int)((_note.NotePosition.Y - next.Y) * ratio));
                        prev = new Point(prev.X + 2, prev.Y);
                        next = new Point(next.X + 2, next.Y);
                        g.DrawBezier(pen, prev, prevcur, curnext, next);
                        prevcur = new Point(prevcur.X + size - 6, prevcur.Y);
                        curnext = new Point(curnext.X + size - 6, curnext.Y);
                        prev = new Point(prev.X + size - 6, prev.Y);
                        next = new Point(next.X + size - 6, next.Y);
                        g.DrawBezier(pen, prev, prevcur, curnext, next);
                    }
                }
            }
            if (previewNote != null && previewVisible)//プレビュー用のノーツを描画
            {
                if (previewNote.NoteStyle == "AirUp" || previewNote.NoteStyle == "AirDown") g.DrawImage(previewNote.NoteImage, new Point(previewNote.NotePosition.X, previewNote.NotePosition.Y - 32));
                else g.DrawImage(previewNote.NoteImage, previewNote.NotePosition);
            }
            if (previewLongNote != null)//プレビュー用ロングノーツを描画
            {
                if (previewLongNote.NoteStyle == "SlideLine")
                {
                    if(previewLongNote.NotePosition.X >= previewNote.NotePosition.X) g.DrawImage(previewLongNote.setNoteImage(), previewNote.NotePosition);
                    else g.DrawImage(previewLongNote.setNoteImage(), new Point(previewLongNote.StartPosition.X, previewNote.NotePosition.Y));
                }
                else g.DrawImage(previewLongNote.setNoteImage(), new Point(previewLongNote.StartPosition.X, previewLongNote.EndPosition.Y));
            }
            g.DrawString((2 * index + 1).ToString().PadLeft(3, '0'), new Font("ＭＳ ゴシック", 8, FontStyle.Bold), Brushes.White, new Rectangle(0, 768 - 5, 30, 10));//MeasureNumber
            g.DrawString((2 * (index + 1)).ToString().PadLeft(3, '0'), new Font("ＭＳ ゴシック", 8, FontStyle.Bold), Brushes.White, new Rectangle(0, 384 - 5, 30, 10));//MeasureNumber
            //if(index == 0) g.DrawString(form1.StartBPM.ToString(), new Font("ＭＳ ゴシック", 8, FontStyle.Bold), Brushes.Lime, new Rectangle(180, 768 - 5, 50, 15));//BPM
            BackgroundImage = storeImage;
            g.Dispose();
            this.Refresh();
            sRoot.setTotalNotes();
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private void deleteNote(ShortNote _note)
        {
            if (_note.PrevNote != null && prevScore != null)
            {
                prevScore.shortNotes.Remove(_note.PrevNote);
                prevScore.update();
            }
            else if(_note.NextNote != null && nextScore != null)
            {
                nextScore.shortNotes.Remove(_note.NextNote);
                nextScore.update();
            }
            if(_note.NotePosition.Y == 2 && nextScore != null)
            {
                foreach(ShortNote __note in nextScore.shortNotes.Reverse<ShortNote>())
                {
                    if(__note.NotePosition.X == _note.NotePosition.X && __note.NotePosition.Y == 770 && __note.NoteStyle == _note.NoteStyle && __note.NoteSize == _note.NoteSize)
                    {
                        nextScore.shortNotes.Remove(__note);
                        nextScore.update();
                        break;
                    }
                }
            }
            else if(_note.NotePosition.Y == 770 && prevScore != null)
            {
                foreach (ShortNote __note in prevScore.shortNotes.Reverse<ShortNote>())
                {
                    if (__note.NotePosition.X == _note.NotePosition.X && __note.NotePosition.Y == 2 && __note.NoteStyle == _note.NoteStyle && __note.NoteSize == _note.NoteSize)
                    {
                        prevScore.shortNotes.Remove(__note);
                        prevScore.update();
                        break;
                    }
                }
            }
            shortNotes.Remove(_note);
            update();
        }

        public void deleteAllNotes()
        {
            shortNotes.Clear();
            dummyNotes.Clear();
            specialNotes.Clear();
            update();
        }

        private Point locationize(Point p)//通常
        {
            int noteX, noteY;
            if (p.X > 160 + leftMargin - 10 * selectedNoteSize) noteX = 160 + leftMargin - 10 * selectedNoteSize;
            else if (p.X < leftMargin) noteX = leftMargin;
            else noteX = ((p.X - leftMargin) / (10 * (16 / selectedGrid))) * (10 * (16 / selectedGrid)) + leftMargin;
            if (p.Y < topMargin) noteY = topMargin;
            else if (p.Y > 768 + topMargin) noteY = 768 + topMargin;
            else noteY = 768 + topMargin + bottomMargin - (int)(((768 + bottomMargin - p.Y) / (768 / (2 * selectedBeat))) * (768 / (double)(2 * selectedBeat))) - topMargin;
            noteX += 1; noteY += -3;//描写の都合上の位置調整

            return new Point(noteX, noteY);
        }

        private Point locationize(Point p, int size)//EditModeなどで
        {
            int noteX, noteY;
            if (p.X > 160 + leftMargin - 10 * size) noteX = 160 + leftMargin - 10 * size;
            else if (p.X < leftMargin) noteX = leftMargin;
            else noteX = ((p.X - leftMargin) / (10 * (16 / selectedGrid))) * (10 * (16 / selectedGrid)) + leftMargin;
            if (p.Y < topMargin) noteY = topMargin;
            else if (p.Y > 768 + topMargin) noteY = 768 + topMargin;
            else noteY = 768 + topMargin + bottomMargin - (int)(((768 + bottomMargin - p.Y) / (768 / (2 * selectedBeat))) * (768 / (double)(2 * selectedBeat))) - topMargin;
            noteX += 1; noteY += -3;//描写の都合上の位置調整

            return new Point(noteX, noteY);
        }
    }
}
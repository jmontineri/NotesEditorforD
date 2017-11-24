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
    public class MusicScore2 : PictureBox
    {
        private static int selectedBeat, selectedGrid, selectedNoteSize, index, tmpLongNoteNumber;
        private static string selectedNoteStyle, selectedEditStatus, selectedAirDirection;
        private Point startPosition, endPosition;
        public Form1 form1;
        public List<ShortNote> shortNotes = new List<ShortNote>();
        public List<ShortNote> dummyNotes = new List<ShortNote>();
        private Bitmap storeImage;
        private ShortNote previewNote, previewLongNote, startNote;
        private MusicScore2 prevScore, nextScore;
        public MusicScore2()
        {
            selectedBeat = 8;
            selectedGrid = 8;
            selectedNoteSize = 4;
            selectedEditStatus = "Add";
            selectedAirDirection = "Center";
            Location = new Point(0, 0);
            Margin = new Padding(10, 7, 10, 7);
            Size = new Size(170, 778);
            BackgroundImage = Properties.Resources.MusicScore;
            storeImage = Properties.Resources.MusicScore;
            previewNote = null;
            previewLongNote = null;
            prevScore = null;
            nextScore = null;

            this.MouseDown += new MouseEventHandler(this_MouseDown);
            this.MouseMove += new MouseEventHandler(this_MouseMove);
            this.MouseUp += new MouseEventHandler(this_MouseUp);
            this.MouseEnter += new EventHandler(this_MouseEnter);
            this.MouseLeave += new EventHandler(this_MouseLeave);
        }

        public MusicScore2 PrevScore
        {
            get { return this.prevScore; }
            set { this.prevScore = value; }
        }

        public MusicScore2 NextScore
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

        public void setNote(string[] _noteData , string dymsVersion)
        {
            Point notePosition;
            int noteSize, longNoteNumber;
            string noteStyle, airDirection;
            if(dymsVersion != "0.1")
            {
                notePosition = new Point(int.Parse(_noteData[2]), int.Parse(_noteData[3]));
                startPosition = new Point(int.Parse(_noteData[4]), int.Parse(_noteData[5]));
                endPosition = new Point(int.Parse(_noteData[6]), int.Parse(_noteData[7]));
                noteSize = int.Parse(_noteData[1]);
                noteStyle = _noteData[0];
                airDirection = _noteData[8];
                longNoteNumber = int.Parse(_noteData[9]);
            }
            else
            {
                notePosition = new Point(int.Parse(_noteData[3]), int.Parse(_noteData[4]));
                startPosition = new Point(int.Parse(_noteData[5]), int.Parse(_noteData[6]));
                endPosition = new Point(int.Parse(_noteData[7]), int.Parse(_noteData[8]));
                noteSize = int.Parse(_noteData[1]) / 10;
                noteStyle = _noteData[0];
                airDirection = _noteData[9];
                longNoteNumber = int.Parse(_noteData[10]);
                if (noteStyle == "AirLine") noteSize = int.Parse(_noteData[11]);
                if (noteStyle == "AirAction") { noteSize += 1; notePosition.X -= 2; }
                if (noteStyle == "AirUp" || noteStyle == "AirDown") notePosition.Y += 32;
            }
            ShortNote shortNote = new ShortNote(this, notePosition, startPosition, endPosition, noteSize, noteStyle, airDirection, longNoteNumber);
            addNote(shortNote);
        }

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            if(selectedEditStatus == "Add")
            {
                ShortNote shortNote;// = null;
                startPosition = locationize(e.Location);
                if (selectedNoteStyle != "AirLine")
                {
                    shortNote = new ShortNote(this, locationize(e.Location), startPosition, startPosition, selectedNoteSize, selectedNoteStyle, selectedAirDirection, -1);
                    //shortNotes.Add(shortNote);
                    addNote(shortNote);
                    startNote = shortNote;
                    if(selectedNoteStyle == "Hold")
                    {
                        shortNote.LongNoteNumber = form1.LongNoteNumber;
                        tmpLongNoteNumber = shortNote.LongNoteNumber;
                        foreach(ShortNote _note in shortNotes)
                        {
                            if(_note.NotePosition == locationize(e.Location) && _note.NoteSize  == selectedNoteSize && _note.NoteStyle == "HoldEnd")
                            {
                                tmpLongNoteNumber = _note.LongNoteNumber;
                                deleteNote(startNote);
                                deleteNote(_note);
                                break;
                            }
                        }
                        previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "HoldLine", selectedAirDirection, 0);
                    }
                    else if (selectedNoteStyle == "Slide")
                    {
                        shortNote.LongNoteNumber = form1.LongNoteNumber;
                        tmpLongNoteNumber = shortNote.LongNoteNumber;
                        for(int i = shortNotes.Count - 1; i >= 0; i--)
                        {
                            if (shortNotes[i].EndPosition == locationize(e.Location) && shortNotes[i].NoteSize == selectedNoteSize && shortNotes[i].NoteStyle == "SlideLine")
                            {
                                tmpLongNoteNumber = shortNotes[i].LongNoteNumber;
                                deleteNote(startNote);
                                break;
                            }
                        }
                        /*
                        foreach (ShortNote _note in shortNotes)
                        {
                            if (_note.EndPosition == locationize(e.Location) && _note.NoteSize  == selectedNoteSize && _note.NoteStyle == "SlideLine")
                            {
                                tmpLongNoteNumber = _note.LongNoteNumber;
                                deleteNote(startNote);
                                break;
                            }
                        }
                        //*/
                        previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "SlideLine", selectedAirDirection, 0);
                    }
                    //addNote(shortNote);
                }
                else
                {
                    previewLongNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, startPosition.Y - 1), selectedNoteSize, "AirLine", selectedAirDirection, 0);
                    tmpLongNoteNumber = form1.LongNoteNumber;
                    foreach (ShortNote _note in shortNotes)
                    {
                        if (_note.EndPosition == locationize(e.Location) && _note.NoteSize == selectedNoteSize && _note.NoteStyle == "AirLine")
                        {
                            tmpLongNoteNumber = _note.LongNoteNumber;
                            //deleteNote(_note);
                            break;
                        }
                    }
                }
                update();
            }
            else if(selectedEditStatus == "Delete")
            {
                bool flg = false;
                for(int i = shortNotes.Count - 1; i >= 0; i--)
                {
                    if (isMouseCollision(shortNotes[i].DestPoints, e.Location))
                    {
                        deleteNote(shortNotes[i]);
                        flg = true;
                        break;
                    }
                }
                if (!flg)
                {
                    for (int i = dummyNotes.Count - 1; i >= 0; i--)
                    {
                        if (isMouseCollision(dummyNotes[i].DestPoints, e.Location))
                        {
                            deleteNote(dummyNotes[i]);
                            break;
                        }
                    }
                }
            }
            form1.setEdited(true);
        }

        private void this_MouseMove(object sender, MouseEventArgs e)
        {
            if (previewNote != null)
            {
                previewNote.NotePosition = locationize(e.Location);
            }
            if(previewLongNote != null)
            {
                if (selectedNoteStyle == "Hold")
                {
                    previewNote.NotePosition = locationize(new Point(startNote.NotePosition.X, e.Y - 1));
                }
                else if (selectedNoteStyle == "Slide")
                {
                    
                }
                else if(selectedNoteStyle == "AirLine")
                {
                    previewNote.NotePosition = locationize(new Point(previewLongNote.StartPosition.X, e.Y - 1));
                }
                if (previewLongNote.StartPosition.Y > e.Y)
                {
                    Cursor.Current = Cursors.Arrow;
                    previewLongNote.EndPosition = locationize(e.Location);
                }
                else Cursor.Current = Cursors.No;
            }
            update();
        }

        private void this_MouseUp(object sender, MouseEventArgs e)
        {
            if(selectedEditStatus == "Add")
            {
                ShortNote shortNote;
                endPosition = locationize(e.Location);
                if (endPosition.Y < 2) endPosition.Y = 2;
                if (endPosition.X < 6) endPosition.X = 6;
                if (endPosition.X > 166 - selectedNoteSize * 10) endPosition.X = 166 - selectedNoteSize * 10; 
                switch (selectedNoteStyle)
                {
                    case "Hold":
                        if (endPosition.Y < startPosition.Y)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "HoldLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, new Point(startPosition.X, endPosition.Y), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "HoldEnd", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            form1.LongNoteNumber++;
                        }
                        else deleteNote(startNote);
                        break;
                    case "Slide":
                        if (endPosition.Y < startPosition.Y)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, endPosition, selectedNoteSize, "SlideLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, endPosition, startPosition, endPosition, selectedNoteSize, "SlideTap", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            form1.LongNoteNumber++;
                        }
                        else deleteNote(startNote);
                        break;
                    case "AirLine":
                        if (endPosition.Y < startPosition.Y)
                        {
                            shortNote = new ShortNote(this, locationize(e.Location), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "AirLine", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            shortNote = new ShortNote(this, new Point(startPosition.X, endPosition.Y), startPosition, new Point(startPosition.X, endPosition.Y), selectedNoteSize, "AirAction", selectedAirDirection, tmpLongNoteNumber);
                            addNote(shortNote);
                            form1.LongNoteNumber++;
                        }
                        break;
                }
                previewLongNote = null;
                update();
            }
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
            if(SelectedEditStatus == "Add")
            {
                previewNote = null;
                update();
            }
        }

        private void addNote(ShortNote shortNote)//, string position)
        {
            /*
            if(position == "Start")
            {
                if (shortNote.StartPosition.Y == 770 && prevScore != null && selectedNoteStyle != "AirLine")
                {
                    ShortNote childShortNote = new ShortNote(prevScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                    childShortNote.StartPosition = new Point(childShortNote.StartPosition.X, childShortNote.StartPosition.Y - 768);
                    childShortNote.EndPosition = new Point(childShortNote.EndPosition.X, childShortNote.EndPosition.Y - 768);
                    childShortNote.NotePosition = new Point(childShortNote.NotePosition.X, childShortNote.NotePosition.Y - 768);
                    childShortNote.IsChildNote = true;
                    childShortNote.ParentNote = shortNote;
                    shortNote.IsParentNote = true;
                    shortNote.ChildNote = childShortNote;
                    shortNotes.Add(shortNote);
                    prevScore.dummyNotes.Add(childShortNote);
                    prevScore.update();
                }
                else if (shortNote.StartPosition.Y == 2 && nextScore != null && SelectedNoteStyle != "AirLine")
                {
                    ShortNote parentShortNote = new ShortNote(nextScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                    parentShortNote.StartPosition = new Point(parentShortNote.StartPosition.X, parentShortNote.StartPosition.Y + 768);
                    parentShortNote.EndPosition = new Point(parentShortNote.EndPosition.X, parentShortNote.EndPosition.Y + 768);
                    parentShortNote.NotePosition = new Point(parentShortNote.NotePosition.X, parentShortNote.NotePosition.Y + 768);
                    parentShortNote.IsParentNote = true;
                    parentShortNote.ChildNote = shortNote;
                    shortNote.IsChildNote = true;
                    shortNote.ParentNote = parentShortNote;
                    nextScore.shortNotes.Add(parentShortNote);
                    dummyNotes.Add(shortNote);
                    nextScore.update();
                }
                else
                {
                    shortNotes.Add(shortNote);
                }
            }
            //*/
            //else if(position == "End")
            //{
                if (shortNote.EndPosition.Y == 770 && prevScore != null)
                {
                    ShortNote childShortNote = new ShortNote(prevScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                    childShortNote.StartPosition = new Point(childShortNote.StartPosition.X, childShortNote.StartPosition.Y - 768);
                    childShortNote.EndPosition = new Point(childShortNote.EndPosition.X, childShortNote.EndPosition.Y - 768);
                    childShortNote.NotePosition = new Point(childShortNote.NotePosition.X, childShortNote.NotePosition.Y - 768);
                    childShortNote.IsChildNote = true;
                    childShortNote.ParentNote = shortNote;
                    shortNote.IsParentNote = true;
                    shortNote.ChildNote = childShortNote;
                    shortNotes.Add(shortNote);
                    prevScore.dummyNotes.Add(childShortNote);
                    prevScore.update();
                }
                else if (shortNote.EndPosition.Y == 2 && nextScore != null)
                {
                    ShortNote parentShortNote = new ShortNote(nextScore, shortNote.NotePosition, shortNote.StartPosition, shortNote.EndPosition, shortNote.NoteSize, shortNote.NoteStyle, shortNote.AirDirection, shortNote.LongNoteNumber);
                    parentShortNote.StartPosition = new Point(parentShortNote.StartPosition.X, parentShortNote.StartPosition.Y + 768);
                    parentShortNote.EndPosition = new Point(parentShortNote.EndPosition.X, parentShortNote.EndPosition.Y + 768);
                    parentShortNote.NotePosition = new Point(parentShortNote.NotePosition.X, parentShortNote.NotePosition.Y + 768);
                    parentShortNote.IsParentNote = true;
                    parentShortNote.ChildNote = shortNote;
                    shortNote.IsChildNote = true;
                    shortNote.ParentNote = parentShortNote;
                    nextScore.shortNotes.Add(parentShortNote);
                    dummyNotes.Add(shortNote);
                    nextScore.update();
                }
                else
                {
                    shortNotes.Add(shortNote);
                }
            //}
            //else
            //{
            //    shortNotes.Add(shortNote);
            //}
            
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
            foreach (ShortNote _note in dummyNotes)
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
            foreach (ShortNote _note in shortNotes)
            {
                if(_note.NoteStyle == "AirUp" || _note.NoteStyle == "AirDown") g.DrawImage(_note.NoteImage, new Point(_note.NotePosition.X, _note.NotePosition.Y - 32));
                else if(_note.NoteStyle == "HoldLine" || _note.NoteStyle == "AirLine") g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                else if(_note.NoteStyle == "SlideLine")
                {
                    if (_note.StartPosition.X >= _note.EndPosition.X) g.DrawImage(_note.NoteImage, _note.EndPosition);
                    else g.DrawImage(_note.NoteImage, new Point(_note.StartPosition.X, _note.EndPosition.Y));
                }
                else g.DrawImage(_note.NoteImage, _note.NotePosition);
            }
            if (previewNote != null)
            {
                if (previewNote.NoteStyle == "AirUp" || previewNote.NoteStyle == "AirDown") g.DrawImage(previewNote.NoteImage, new Point(previewNote.NotePosition.X, previewNote.NotePosition.Y - 32));
                else g.DrawImage(previewNote.NoteImage, previewNote.NotePosition);
            }
            if (previewLongNote != null)
            {
                if (previewLongNote.NoteStyle == "SlideLine")
                {
                    if(previewLongNote.NotePosition.X >= previewNote.NotePosition.X) g.DrawImage(previewLongNote.setNoteImage(), previewNote.NotePosition);
                    else g.DrawImage(previewLongNote.setNoteImage(), new Point(previewLongNote.StartPosition.X, previewNote.NotePosition.Y));
                }
                else g.DrawImage(previewLongNote.setNoteImage(), new Point(previewLongNote.StartPosition.X, previewLongNote.EndPosition.Y));
            }
            BackgroundImage = storeImage;
            g.Dispose();
            this.Refresh();
            form1.setTotalNotes();
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private void deleteNote(ShortNote _note)
        {
            if (_note.IsParentNote && prevScore != null)
            {
                prevScore.dummyNotes.Remove(_note.ChildNote);
                shortNotes.Remove(_note);
                prevScore.update();
            }
            else if(_note.IsChildNote && nextScore != null)
            {
                nextScore.shortNotes.Remove(_note.ParentNote);
                dummyNotes.Remove(_note);
                nextScore.update();
            }else
            {
                shortNotes.Remove(_note);
            }
            update();
        }

        public void deleteAllNotes()
        {
            shortNotes.Clear();
            dummyNotes.Clear();
            update();
        }

        private Point locationize(Point p)
        {
            int noteX, noteY;
            if (p.X > 165 - 10 * selectedNoteSize) noteX = 165 - 10 * selectedNoteSize;
            else if (p.X < 5) noteX = 5;
            else noteX = ((p.X - 5) / (10 * (16 / selectedGrid))) * (10 * (16 / selectedGrid)) + 5;
            if (p.Y < 5) noteY = 5;
            else noteY = 778 - ((778 - p.Y - 5) / (768 / (2 * selectedBeat))) * (768 / (2 * selectedBeat)) - 5;
            noteX += 1; noteY += -3;//描写の都合上の位置調整

            return new Point(noteX, noteY);
        }
    }
}
using System;
using SimpleJSON;


public class Partition
{
    string title;
    int notesCount;
    Note[] notes;
    int currentRead;

    public Partition(string title, Note[] notes, int notesCount)
    {
        this.title = title;
        this.notes = notes;
        this.notesCount = notesCount;
    }

    public Partition(JSONNode node)
    {
        this.title = node["title"];
        this.notesCount = node["notesCount"].AsInt;
        this.notes = new Note[this.notesCount];
        JSONArray notesArray = node["notes"].AsArray;
        for (int i = 0; i < this.notesCount; i++)
        {
            JSONNode note = notesArray[i];
            this.notes[i] = new Note(note["note"], note["longueur"]);
        }
    }

    public void StartReading()
    {
        currentRead = 0;
    }

    public Note ReadNextNote()
    {
        if (currentRead == notesCount)
            StartReading();

        Note next = this.notes[currentRead];
        currentRead++;

        return next;
    }

    public Note GetNoteAt(int index)
    {
        return this.notes[index];
    }
}

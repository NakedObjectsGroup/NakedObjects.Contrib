using System;
using Cluster.Documents.Api;
using NakedObjects;
namespace Cluster.Documents.Impl
{
    public class Note : Document
    {
        #region LifeCycle Methods

        public void Created()
        {
            Status = NoteStatus.Active;
        }
        public override void Persisting()
        {
            base.Persisting();
            AddUserAndTimeStampToText();
        }

        private void AddUserAndTimeStampToText()
        {
            string user = UserFinder.CurrentUser().UserName;
            Text += "\n" + User.UserName + ", " + Clock.Now();
        }
        #endregion

        #region Title
        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append("Notes");
            return t.ToString();
        }
        #endregion

        #region Properties  -  additional and overrides

        [Disabled]
        public virtual NoteStatus Status { get; set; }
      

        public override string DisableText()
        {
            return Container.IsPersistent(this) ? "Use 'Add To Note' action.": null;
        }

        public override DocumentType DefaultType()
        {
            return DocumentType.Note;
        }

        [MultiLine(NumberOfLines = 10), MemberOrder(100)]
        public override string Text { get; set; }

        #endregion

        #region AddToNote (Action)

        [MemberOrder(10)]
        public void AddToNote(string text )
        {
            Text += "\n\n" + text;
            AddUserAndTimeStampToText();

        }

        public string DisableAddToNote()
{
  var rb = new ReasonBuilder();
  rb.AppendOnCondition(Status == NoteStatus.Finished, "Cannot add to a Finished note. Start a new one instead");
  return rb.Reason;
}

        #endregion  
    
        #region Finish (Action)
        [MemberOrder(20)]
        public void FinishThisNote()
        {
            Status = NoteStatus.Finished;
        }

        public bool HideFinishThisNote()
        {
            return Status == NoteStatus.Finished;
        }

        #endregion
    }

    public enum NoteStatus
    {
        Active, Finished
    }
}

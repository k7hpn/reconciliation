namespace Reconciliation.Models.Insta
{
    public class MediaFile
    {
        public string SourceFile { get; set; }
        public Media[] Photos { get; set; }
        public Media[] Videos { get; set; }
    }
}

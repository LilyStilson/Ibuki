using System;
using System.Collections.Generic;
using System.Text;

namespace IbukiBooruLibrary.Booru {
    public class BooruData {
        public string BaseURL { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Uri BaseURI => new Uri(BaseURL);

        public Post post { get; set; }
        public PostEndpoints postEndpoints { get; set; }

        public Comment comment { get; set; }
        public CommentEndpoints commentEndpoints { get; set; }

        public Note note { get; set; }
        public NoteEndpoints noteEndpoints { get; set; }

        public ArtistCommentary artistCommentary { get; set; }
        public ArtistCommentaryEndpoints artistCommentaryEndpoints { get; set; }

        public User user { get; set; }
        public UserEndpoints userEndpoints { get; set; }
    }
}

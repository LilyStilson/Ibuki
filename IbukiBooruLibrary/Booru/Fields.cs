using System;

namespace IbukiBooruLibrary.Booru {
    public class Post {
        /// Required priority
        public string ID { get; set; } = "id";
        public string PreviewImageURL { get; set; } = "preview_file_url";
        public string LargeImageURL { get; set; } = "large_file_url";

        /// High priority
        public string SourceURL { get; set; } = "source";
        public string GeneralTags { get; set; } = "tag_string_general";

        /// Medium priority
        public string CopyrightTags { get; set; } = "";
        public string CharacterTags { get; set; } = "";
        public string SpeciesTags { get; set; } = "";
        public string ArtistTags { get; set; } = "";
        public string LoreTags { get; set; } = "";
        public string MetaTags { get; set; } = "";
        public string TagsSeparator { get; set; } = " ";
        public string IsFavorited { get; set; } = "";

        /// Low priority
        public string ParentID { get; set; } = "";
        public string HasChildren { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string Rating { get; set; } = "";
        public string FavoritesCount { get; set; } = "";
        public string UpScore { get; set; } = "";
        public string DownScore { get; set; } = "";
    }

    public class Comment {
        public string ID { get; set; } = "id";
        public string PostID { get; set; } = "post_id";
        public string CreatorID { get; set; } = "creator_id";
        public string Body { get; set; } = "body";

        public string CreatedAt { get; set; } = "";

        public string UpdatedAt { get; set; } = "";
        public string UpdaterID { get; set; } = "";
    }

    public class Note {
        public string ID { get; set; } = "id";
        public string PostID { get; set; } = "post_id";
        public string Body { get; set; } = "body";
        public string PositionX { get; set; } = "x";
        public string PositionY { get; set; } = "y";
        public string Width { get; set; } = "width";
        public string Height { get; set; } = "height";

        public string CreatedAt { get; set; } = "";

        public string UpdatedAt { get; set; } = "";
    }

    public class ArtistCommentary {
        public string ID { get; set; } = "id";
        public string PostID { get; set; } = "post_id";
        public string Title { get; set; } = "original_title";
        public string Description { get; set; } = "original_description";

        public string TranslatedTitle { get; set; } = "";
        public string TranslatedDescription { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string UpdatedAt { get; set; } = "";
    }

    public class User {
        public string ID { get; set; } = "id";
        public string Username { get; set; } = "name";

        public string LimitPerPage { get; set; } = "";
    }
}

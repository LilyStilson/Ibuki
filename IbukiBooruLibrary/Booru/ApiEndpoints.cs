using System;
using System.Collections.Generic;

namespace IbukiBooruLibrary.Booru {
    public static class BooruEndpoints {
        public static string ParseEndpointLink(string str, Dictionary<string, string> args) {
            string result = str;
            foreach (KeyValuePair<string, string> arg in args) {
                if (result.Contains($"{{{arg.Key}}}")) {
                    result = result.Replace($"{{{arg.Key}}}", arg.Value);
                }
            }
            if (str == result) {
                throw new ArgumentException("Provided dictionary of arguments was not found in the source string");
            }
            return result;
        }
    }

    public class PostEndpoints {
        /// <summary>
        /// Params (in order): limit, page, search
        /// </summary>
        public string GET_Posts { get; set; } = "/posts.json?limit={limit}&page={page}&tags={tags}";
        /// <summary>
        /// Params (in order): id
        /// </summary>
        public string GET_Post { get; set; } = "/post/{id}.json";  
    }

    public class CommentEndpoints {
        /// <summary>
        /// Params (in order): post_id
        /// </summary>
        public string GET_CommentsForPost { get; set; } = "/comments.json?search[post_id]={0}";
        /// <summary>
        /// Params (in order): id
        /// </summary>
        public string GET_Comment { get; set; } = "/comments.json?id={0}"; 
    }

    public class NoteEndpoints {
        /// <summary>
        /// Params (in order): post_id
        /// </summary>
        public string GET_NotesForPost { get; set; } = "/notes.json?search[post_id]={0}";
        /// <summary>
        /// Params (in order): id
        /// </summary>
        public string GET_Note { get; set; } = "/notes/{0}.json";
    }

    public class ArtistCommentaryEndpoints {
        /// <summary>
        /// Params (in order): post_id
        /// </summary>
        public string GET_CommentaryForPost { get; set; } = "/posts/{0}/artist_commentary.json";
        /// <summary>
        /// Params (in order): id
        /// </summary>
        public string GET_Commentary { get; set; } = "/artist_commentaries/{0}.json";
    }

    public class UserEndpoints {
        /// <summary>
        /// Params (in order): username, api_key
        /// </summary>
        public string GET_LoginAPIKey { get; set; } = "/profile.json?login={0}&api_key={1}";
        /// <summary>
        /// Params (in order): username, password
        /// </summary>
        public string GET_LoginPassword { get; set; } = "";
        /// <summary>
        /// Params (in order): username
        /// </summary>
        public string GET_UserByName { get; set; } = "/users.json?search[name]={0}";
        /// <summary>
        /// Params (in order): id
        /// </summary>
        public string GET_User { get; set; } = "/users/{0}.json";
    }
}

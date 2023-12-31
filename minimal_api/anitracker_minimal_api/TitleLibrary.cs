﻿namespace anitracker_minimal_api
{
    public static class TitleLibrary
    {
        /// <summary>
        /// Searches for <paramref name="title"/> inside the titles table
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="title">The title to search for</param>
        /// <returns><see cref="bool">true</see> if title is found, <see cref="bool">false</see> otherwise </returns>
        public static TitleItem? Find(TitleDb _context, string title)
        {
            if (!_context.Titles.Any()) return null;
            return _context.Titles.FirstOrDefault(db_title => db_title.Title == title);
        }
        /// <summary>
        /// Adds a single <paramref name="title"/> to the titles table
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="title">The title to add</param>
        /// <param name="poster">The poster image URL</param>
        /// <returns>The added title</returns>
        public static async Task<TitleItem> Add(TitleDb _context, string title, string? poster)
        {
            return (await AddRange(_context, new List<TitleItem>() { new TitleItem() { Title = title, Poster = poster } })).FirstOrDefault(new TitleItem { ID = -1, Title = title, Poster = poster });
        }
        /// <summary>
        /// Adds miltiple <paramref name="titles"/> to the titles table
        /// </summary>
        /// <param name="_context">The database context</param>
        /// <param name="titles">The list of titles to add</param>
        /// <returns>The added list</returns>
        public static async Task<List<TitleItem>> AddRange(TitleDb _context, List<TitleItem> titles)
        {
            if (!titles.Any()) return new List<TitleItem>();

            var new_titles = titles.Where(item => Find(_context, item.Title) is null).ToList();

            if (!new_titles.Any()) return new List<TitleItem>();

            _context.Titles.AddRange(new_titles);

            await _context.SaveChangesAsync();

            return new_titles;
        }
    }
}

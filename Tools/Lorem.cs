using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touch.Tools
{
    /// <summary>
    /// A static class that returns nonsense faux-Latin text, that may be used as a placeholder on web pages and the like.
    /// </summary>
    public static class Lorem
    {
        static List<string> Paragraphs = new List<string>();

        /// <summary>
        /// Returns a number of words of faux-Latin text, beginning with the words "Lorem Ipsum". 
        /// </summary>
        /// <param name="words">The number of words to be returned: 26 by default.</param>
        /// <returns>Faux-Latin Nonsense</returns>
        public static string Ipsum(int words = 26)
        {
            return Ipsum(0, words);
        }

        /// <summary>
        /// Returns a number of words of faux-Latin text, from one of five possible "paragraphs":
        /// numbered 0 to 5.  If you choose paragraph 0, the text will begin with "Lorem Ipsum".
        /// If the chosen paragraph does not contain enough words, words from the next paragraph 
        /// will be added to fill it.  However no line-feeds are included.
        /// </summary>
        /// <param name="para">The paragraph to start with.</param>
        /// <param name="words">The number of words to be returned.</param>
        /// <returns>Faux-Latin Nonsense</returns>
        public static string Ipsum(int para, int words)
        {
            StringBuilder LoremIpsum = new StringBuilder();
            while (true)
            {
                para %= Paragraphs.Count;
                var wordList = new List<string>(Paragraphs[para].Split(new char[] { ' ' }));
                for (int i = 0; i < wordList.Count; i++)
                {
                    LoremIpsum.Append(wordList[i]);
                    if (--words == 0)
                        return LoremIpsum.ToString();

                    LoremIpsum.Append(" ");
                }
                para += 1;
            }
        }

        /// <summary>
        /// This static constructor simply pre-loads the Text list with 5 paragraphs of at least 100 words each
        /// </summary>
        static Lorem() {
            Paragraphs.Add("Lorem ipsum dolor sit amet, consectetur adipiscing " + 
                "elit. Nulla ac venenatis erat. Donec vitae neque tellus. Morbi non  " + 
                "justo consectetur, malesuada magna sit amet, congue quam. " +  // 26 words
                "Ut faucibus ipsum odio. Nam dignissim magna ac eros " +
                "ullamcorper vestibulum. Donec in risus sit amet felis lacinia " +
                "pretium. Nunc tincidunt sapien et nunc dapibus pulvinar. Ut " +
                "aliquet luctus justo, sed commodo mi consectetur a. Nam nec " +
                "metus placerat, consectetur purus eu, tempor mi. Vestibulum " +
                "venenatis dignissim tellus quis lobortis. Nunc quis tempor magna. " +
                "Aenean consequat nunc lectus, ut sollicitudin tortor viverra id. " +
                "Nullam maximus sapien et laoreet tempus. Duis ullamcorper semper " +
                "massa, vitae lobortis enim molestie at.");

            Paragraphs.Add("Nulla ullamcorper elementum leo sed volutpat. Aliquam " +
                "sed ornare libero, vitae viverra ligula. Donec imperdiet leo " +
                "et velit congue luctus. Fusce dapibus elit nisl, dapibus " +
                "sollicitudin dui fringilla eget. In sit amet aliquam diam. " +
                "Praesent nec eros et enim molestie vulputate. Ut non lectus " +
                "ac leo volutpat aliquam vitae eget mauris. Maecenas nec " +
                "condimentum elit, nec convallis quam. Vivamus eleifend, erat " +
                "sit amet efficitur lacinia, massa diam tempus tortor, at " +
                "rhoncus nibh ligula fringilla tellus. Donec mattis magna at " +
                "quam commodo, at aliquam metus ultricies. Cras semper, leo " +
                "nec pharetra mattis, risus lacus semper arcu, nec bibendum " +
                "neque ipsum porttitor diam. Nulla rhoncus nisl eu tortor " +
                "sollicitudin sagittis. In ultricies ipsum non dignissim " +
                "tincidunt. Vestibulum pellentesque condimentum lobortis.  " +
                "Suspendisse placerat mauris et consequat accumsan.");

            Paragraphs.Add("Vivamus ullamcorper nulla et congue aliquam. Donec sodales " +
                "malesuada tellus, ac tristique turpis iaculis ac. Fusce sit " +
                "amet fringilla elit. Morbi eros augue, efficitur maximus " +
                "dignissim vitae, placerat id nisi. Suspendisse dictum elit " +
                "id nunc scelerisque commodo. Curabitur tincidunt, justo at " +
                "auctor efficitur, sem ante eleifend enim, porttitor gravida " +
                "nisi urna quis diam. In mollis odio sit amet nisl " +
                "sollicitudin, in pretium mi malesuada. Donec tincidunt sagittis " +
                "felis, quis porta enim. Integer faucibus eros ut diam " +
                "pellentesque cursus. Pellentesque at posuere erat, quis " +
                "hendrerit eros. Curabitur finibus arcu eget nibh elementum " +
                "fringilla. Morbi ac iaculis magna, sed volutpat lacus. Duis " +
                "gravida luctus rutrum.");

            Paragraphs.Add("Interdum et malesuada fames ac ante ipsum primis in " +
                "faucibus. Nunc aliquam egestas massa, sit amet ornare eros " +
                "accumsan non. Nam viverra elementum nulla a pretium. In " +
                "dignissim ante non nisi semper, eu mollis urna venenatis. " +
                "Nam id ante ut est aliquam consectetur. Sed porta, lacus " +
                "quis ullamcorper sodales, ligula nisl mollis nisi, vitae " +
                "luctus risus nisi sed metus. Proin a purus ut massa " +
                "pellentesque ultrices. Etiam pulvinar orci ut magna lobortis " +
                "accumsan. Nam cursus egestas arcu sit amet dignissim. " +
                "Aliquam eu hendrerit massa, id lacinia lectus. Proin " +
                "molestie nibh leo, eu dapibus turpis hendrerit ac. Sed " +
                "ac hendrerit metus, vel faucibus enim. Quisque facilisis " +
                "ipsum quis urna ultrices, et imperdiet nisi sollicitudin. " +
                "Cras tempor dui risus, quis pulvinar magna ultrices a. " +
                "Fusce id velit gravida, mattis elit at, imperdiet urna.");

            Paragraphs.Add("Donec id ullamcorper odio. Donec elit lorem, finibus " +
                "ac lobortis eget, dapibus id quam. Morbi egestas porttitor " +
                "nunc in ultricies. Vivamus fringilla in justo vel faucibus. " +
                "Phasellus eget sollicitudin tortor. Aenean in diam dolor. " +
                "Phasellus consectetur, velit sed vulputate scelerisque, felis " +
                "est facilisis augue, id sagittis nulla urna sit amet " +
                "ipsum. Nam posuere turpis eget sem faucibus sodales. " +
                "Phasellus dui turpis, bibendum a libero et, varius varius " +
                "dui. Sed scelerisque eros ac tempus consequat. Pellentesque " +
                "non tellus eget dolor finibus bibendum in porttitor turpis. " +
                "Etiam id semper elit. Nunc vel lorem id velit pellentesque " +
                "tempor. Curabitur at orci tristique, cursus nibh at, " +
                "congue ligula.");
        }


    }
}
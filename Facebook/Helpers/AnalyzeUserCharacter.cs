using Facebook.Models;
using Facebook.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Helpers
{
    public class AnalyzeUserCharacter
    {
        public static string GetCharacteristics(List<string> contents, List<DataKeys> dataKeys)
        {
            int score = 0;
            var characters= dataKeys.Select(s => s.Character).Distinct().ToArray();
            List<KeyPair> KeyPairs = new List<KeyPair>();
            for(int i=0;i<characters.Length;i++)
            {
                score = 0;
                foreach (var keys in dataKeys.Where(d => d.Character == characters[i]))
                {                    
                    for(int j=0;j< contents.Count();j++)
                    {
                        if (contents[j].ToLower().Contains(keys.Key))
                        {
                            score++;
                        }
                    }                    
                }
                KeyPair keyPair = new KeyPair
                {
                    Character = characters[i],
                    Score = score
                };
                KeyPairs.Add(keyPair);
            }
            if (KeyPairs.OrderByDescending(s => s.Score).Select(s => s.Score).FirstOrDefault() > 0)
                return KeyPairs.OrderByDescending(s => s.Score).Select(s => s.Character).FirstOrDefault();
            else
                return "Un-Identified";
        }
        public class KeyPair
        {
            public string Character { get; set; }
            public int Score { get; set; }
        }
    }
}

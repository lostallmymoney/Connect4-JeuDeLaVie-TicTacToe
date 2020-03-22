using System.Collections.Generic;

namespace JeuDeLaVie
{
    public static class StructureManager
    {
        public static List<StructureTemplate> StructureTemplates { get; }
        public static List<StructureTemplate> StructureTexture { get; }
        static StructureManager()
        {
            StructureTemplates = new List<StructureTemplate>();
            StructureTexture = new List<StructureTemplate>();

            StructureTemplates.Add(new StructureTemplate(new bool[3, 3] {
                { true, true, true },
                { true, false, false },
                { false, true, false }},
                "V1"));
            StructureTemplates.Add(new StructureTemplate(new bool[4, 5] {
                { false, true, true, true, true},
                { true, false, false, false, true },
                { false, false, false, false, true },
                { true, false, false, true, false }},
                "V2"));
            StructureTemplates.Add(new StructureTemplate(new bool[4, 6] {
                { false, false, false, true, true, false },
                { true, true, true, false, true, true },
                { true, true, true, true, true, false },
                { false, true, true, true, false, false} },
                "V3"));
            StructureTemplates.Add(new StructureTemplate(new bool[4, 7] {
                { false, false, false, false, true, true, false },
                { true, true, true, true, false, true, true },
                { true, true, true, true, true, true, false },
                { false, true, true, true, true, false, false} },
                "V4"));

            StructureTemplates.Add(new StructureTemplate(new bool[37, 31] {
                { false,false,false,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,true,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,true,false,true,true,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,true,true,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,true,false,false,false,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,true,false,false,false,false,false,true,false,true,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { true,false,false,false,false,false,true,false,false,false,true,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { true,false,false,false,false,true,true,false,false,false,true,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,true,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,true,false,true,false,true,false,true,false,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,true,false,true,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,true,true,false,false,false,false,false,true,false,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,true,false,true,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,true,true,false,true,true,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,true,false,true,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,true,true,false,true,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,true,true,true,true,false,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,true,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,true,true,true,false,false,false,false,false,true,false,false,true,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,true,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,true,true,true,false,false,true,false,true,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,true,true,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,true,false,false,true,false,false,true,false,false,true,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,true,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,true,true,true,false,false,false,true,false,true,true,true,false,true,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,true,true,false,false,false,false,false,false,true,false,true,false,true,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,true,false,false,false,false,false,false,false,false,false},
                { false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,true,true,false,false,false,false},
                
                
                },
                "SR"));

            StructureTemplates.Add(new StructureTemplate(new bool[9, 36] {
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false },
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, false },
                { false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, true, true },
                { false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, true, true },
                { true, true, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                { true, true, false, false, false, false, false, false, false, false, true, false, false, false, true, false, true, true, false, false, false, false, true, false, true, false, false, false, false, false, false, false, false, false, false, false },
                { false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false },
                { false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                { false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false }},
                "C1"));

            StructureTemplates.Add(new StructureTemplate(new bool[21, 33] {
                { true, true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { true, true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, true, true, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, true, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, true, true},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, false, true, false, false, false, true, true},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false}
            }, 
            "C2"));


            StructureTexture.Add(new StructureTemplate(new bool[5, 5] {
                { false, true, false, false, false},
                { false, false, true, false, false},
                { false, false, false, true, false},
                { false, false, true, false, false},
                { false, true, false, false, false}
            }
            , "arrow"));

        }

        
    }
}

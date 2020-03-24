using System.Collections.Generic;
using System.Diagnostics;

namespace JeuDeLaVie
{
    public class StructureManager
    {
        public List<StructureTemplate> StructureTemplates { get; }
        public List<StructureTemplate> StructureTemplatesNature { get; }
        public List<StructureTemplate> StructureTexture { get; }

        public StructureManager()
        {
            StructureTemplates = new List<StructureTemplate>();
            StructureTexture = new List<StructureTemplate>();
            StructureTemplatesNature = new List<StructureTemplate>();

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

            const string SIR_ROBIN = "....OO.............................O..O...........................O...O............................OOO........................OO......OOOO...................O.OO....OOOO..................O....O......OOO.................OOOO....OO...O...............O.........OO....................O...O...............................OOO..OO..O.................OO.......O....O...........................O.OO........................OO......O.......................OO.OOO.O......................OO...O..O......................O.O..OO........................O..O.O.O.......................OOO......O......................O.O.O...O.........................OO.O.O......................O......OOO....................................................O.........O....................O...O......O....................O.....OOOOO....................OOO................................OO..........................OOO..O.......................O.OOO.O.......................O...O..O........................O....OO.OOO......................OOOO.O....OO...................O.OOOO....OO.........................O...............................O..OO..........................OO..............................OOOOO..............................OO.......................OOO......O......................O.O...O.O.....................O...O...O......................O...OO........................O......O.OOO....................OO...O...OO.....................OOOO..O..O.......................OO...O........................O..............................OO.O..........................O.............................OOOOO..........................O....O........................OOO.OOO........................O.OOOOO........................O................................O..........................O....OOOO..........................OOOO.OO.....................OOO....O..............................O.O................................O..........................O..OO...........................OOO.........................OO............................OOO.....O.........................OO..O.O.....................O..OOO.O.O......................OO.O..O..........................O.O..OO..........................OO.........................OOO....O.......................OOO....O........................OO...OOO........................OO.OO...........................OO.............................O............................................................OO...............................O....";
            bool[,] b = new bool[31, 79];
            for (int y = 0; y<79; y++)
            {
                for (int x = 0; x < 31; x++)
                {
                    if (SIR_ROBIN[31 * y + x] == 'O')
                    {
                        b[x, y] = true;
                    }
                    else
                    {
                        b[x, y] = false;
                    }
                }
            }
            StructureTemplates.Add(new StructureTemplate(b, "SR"));

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
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false},
                { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false}
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

            StructureTemplates.ForEach(s =>
            {
                if (s.Id[0] != 'V' && s.Id[0] != 'S' && s.Id!="arrow")
                    StructureTemplatesNature.Add(s);
            });


        }


        
    }
}

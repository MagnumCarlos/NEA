namespace NEA
{
    public class Item
    {
        string Name;
        bool CanPickUp;
        public Item(string NAME, bool CANPICKUP)
        {
            this.Name = NAME;
            this.CanPickUp = CANPICKUP;
        }
        public static List<Item> LoadItems(string FileName)
        {
            if(Program.CheckForFile(FileName))
            {
                StreamReader Reader = new StreamReader(FileName);
                List<Item> items = new List<Item>();
                using (Reader)
                {
                    int NumberOfLines = File.ReadAllLines(FileName).Length;
                    for (int i = 0; i < NumberOfLines / 2; i++)
                    {
                        string ItemName = Reader.ReadLine();
                        string CanPickUp = Reader.ReadLine();
                        bool CANPICKUP = false;
                        if (CanPickUp == "true")
                        {
                            CANPICKUP = true;
                        }
                        else if (CanPickUp == "false")
                        {
                            CANPICKUP = false;
                        }
                        Item item = new Item(ItemName, CANPICKUP);
                        items.Add(item);
                    }
                }
                return items;
            }
            return new List<Item>();
        }
    }
}

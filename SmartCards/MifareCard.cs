namespace Touch.SmartCards
{
    public class MifareCard
    {
        protected string _cardUID = "";
        protected string _SAK = "";
        protected byte[][] _keyA = new byte[16][]; // [Sector][6]
        protected byte[][] _keyB = new byte[16][]; // [Sector][6]
        protected byte[][] _data = new byte[64][]; // [block][16]

        public MifareCard() { }

        public MifareCard (string UID)
        {
            _cardUID = UID;
            for (var i=0; i < 16; i++)
            {
                _keyA[i] = new byte[6];
                _keyB[i] = new byte[6];
            }
        }

        public string UID
        {
            get { return _cardUID; }
            set { _cardUID = value; }
        }

        public string SAK
        {
            get { return _SAK; }
            set { _SAK = value; }
        }


        public byte[] KeyA(int sector)
        {
            if (sector > -1 && sector < _keyA.Length)
                return _keyA[sector];
            else
                return null;
        }

        public byte[] KeyB(int sector)
        {
            if (sector > -1 && sector < _keyB.Length)
                return _keyB[sector];
            else
                return null;
        }

        public void SetKeyA(int sector, byte[] key)
       {
           if (sector > -1 && sector < _keyA.Length && key.Length == 6)
               _keyA[sector] = key;
       }

       public void SetKeyB(int sector, byte[] key)
       {
           if (sector > -1 && sector < _keyB.Length && key.Length == 6)
               _keyB[sector] = key;
       }

       public byte[] Data(int block)
       {
           if (block > -1 && block < _data.Length)
               return _data[block];
           else
               return null;
       }

       public void SetData(int block, byte[] data)
       {
           if (block > -1 && block < _keyB.Length && data.Length == 16)
               _data[block] = data;
       }



    }
}

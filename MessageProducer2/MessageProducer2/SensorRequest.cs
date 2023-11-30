public class SensorRequest
	{
       public int ID { get; set; }

        public int ID_Dev_Inst { get; set; }

        public float Value { get; set; }

        public string Time { get; set; }


        //reference to parent

        public string ID_User { get; set; }

        public SensorRequest()
		{
		}
	}
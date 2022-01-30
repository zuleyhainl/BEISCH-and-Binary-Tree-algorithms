using System;

namespace FileOrganizationHW
{
    //Table Size degerini asal sayı olmasından ve max packing factor degerini geçmemesinden dolayı 997 aldım
    //I took table size as 997
    public class Node
    {
        public int data;
        public Node next;
        public Node(int d)
        {
            data = d;
            next = null;
        }
    }
    class Bidirectional
    {
        public int begin;
        public int end;
        public int[] probe;
        public Node[] records;//table

        public void put_array(Node[] records, Node node, int d)//gelen düğüm tabloya eklenir
        {
            records[d] = node;
        }
        public void linking(Node first, Node sec)//early insertion ile linking yapılır
        {
            Node node = first.next;
            first.next = sec;
            sec.next = node;
        }
        public Node find(int d, int size, Node[] records)//tablodan key aranır, bulunmazsa null döner
        {
            int loc = d % size;
            Node node = records[loc];
            while (node.data != d)
            {
                node = node.next;
            }
            return node;
        }
        public int find_probe(int d, int size, Node[] records)//linkler takip edilerek probe hesaplanır
        {
            int probe = 1;
            int loc = d % size;
            Node node = records[loc];
            while (node.data != d)
            {
                node = node.next;
                probe++;
            }
            return probe;
        }
        public void createProbe(int[] probe, Node[] records, int size)
        {
            for (int i = 0; i < probe.Length; i++)
            {
                if (probe[i] != 1)//1 ise bakma
                {
                    if (records[i] != null)
                        probe[i] = find_probe(records[i].data, size, records);
                }
            }
        }
        public void writeConsole(Node[] records, int size, int[] probe)
        {
            //ekrana yazdır
            Console.WriteLine("\nBEISCH\n");
            Console.WriteLine("{0}\t{1,-15}\t{2,10}\t{3,-10}", "i", "Key", "Next Index", "Probe");
            for (int i = 0; i < records.Length; i++)
            {
                Console.Write("{0}\t", i);
                if (records[i] == null)//kendisi de nekti de null
                {
                    Console.Write("{0,-15}\t{1,-10}", "Null", "Null");
                }                  
                else
                {
                    Console.Write("{0,-15}", records[i].data);//olan keyi yazdır
                    if (records[i].next != null)//nextinde eleman varsa onu da yazdır
                    {
                        //Console.Write("{0,-10}", (find(records[i].next.data, size, records)).data);
                        Console.Write("\t{0,-10}", Array.IndexOf<Node>(records, records[i].next));
                        
                    }                       
                    else
                        Console.Write("\t{0,-10}", "Null");
                }

                Console.WriteLine("\t{0,-10}", probe[i]);
            }
        }
        public void createBidirectional(int[] keysArray,int _size)
        {
            Node node;
            bool begin_or_end = true;//true:son//false:bas
            int sayi;//key degerini tutar
            int size = _size;//packing factor %90 olacağından table ın max size ı 1000 olmalı --> 1001
            records = new Node[size];
            probe = new int[size];
            begin = 0;
            end = size - 1;

            for (int i = 0; i < keysArray.Length; i++)
            {
                sayi = keysArray[i];
                int loc = sayi % size;
                
                if (records[loc] == null)//boşsa direk yerleştir
                {
                    node = new Node(sayi);
                    put_array(records, node, loc);
                    probe[loc] = 1;//ilk yerinde probe 1
                }

                else
                {
                    node = new Node(sayi);
                    if (begin_or_end)//sıra sondan eleman eklemede
                    {
                        Node is_empty = records[end];
                        while (is_empty != null)//dolu
                        {
                            end--;
                            is_empty = records[end];
                        }
                        put_array(records, node, end);
                        //nodeları birbirine bagla
                        linking(records[loc], node);
                        begin_or_end = false;
                        end--;
                    }
                    else//sıra bastan eleman eklemede
                    {
                        Node is_empty = records[begin];
                        while (is_empty != null)
                        {
                            begin++;
                            is_empty = records[begin];
                        }
                        put_array(records, node, begin);
                        linking(records[loc], node);
                        begin_or_end = true;
                        begin++;
                    }
                }
            }
            //probeları ayarla
            createProbe(probe, records, size);
            writeConsole(records, size, probe);//ekrana yazdır
        }
    }
    class Heap
    {
        public int[] heapArray;
        public int maxSize;//maximum capacity = tablesize
        public int currentSize;//anlık ne kadar dolu//her eklemede 1 artar//silmede 1 azalır
        public int[] probes;
        public int[] records;
        public void CreateHeap(int size)//constructer
        {
            maxSize = size;
            heapArray = new int[maxSize];
            currentSize = 0;//arrayin başlangıçtaki uzunluğu 0
        }
        public int Parent(int i)//parentın indexini döndürür
        {
            if ((i - 1) / 2 >= 0)
                return (i - 1) / 2;
            else
                return -1;
        }
        public bool isRightChild(int i)//sağ çocuksa true döner
        {
            if (i % 2 == 0)
            {
                return true;
            }
            else
                return false;
        }
        public void createBinary(int[] keysArray, int _size)
        {
            int key;//key degerini tutar
            int size = _size;
            records = new int[size];
            probes = new int[size];

            CreateHeap(32000);//

            for (int i = 0; i < keysArray.Length; i++)//dizi arrayindeki keyleri records tablosuna yerleştirmeye çalışıyoruz
            {
                key = keysArray[i];
                int loc = key % size;//ilk hashleme mod alma
                if (records[loc] == 0)//boşsa direk yerleştir
                {
                    records[loc] = key;
                }
                else//tree yapısında boş yer aramaya başla
                {
                    currentSize = 0;
                    heapArray[currentSize] = loc;//heapin 0.indexine loc değerini yaz//burası zaten dolu
                    while (records[loc] != 0)//yer olmadığı sürece çalış
                    {
                        currentSize++;
                        if (!(isRightChild(currentSize)))//sol çocuk//önce adres hesapla, en son boş mu diye kontrol et//tree.currentSize % 2 != 0
                        {
                            //adres hesaplama, parent sağ çocuk olana kadar yukarı git
                            int parent_index = Parent(currentSize);//
                            while (!(isRightChild(parent_index)))//parent sağ çocuk olana kadar yukarı//parent_index % 2 != 0
                            {
                                parent_index = Parent(parent_index);
                            }
                            if (isRightChild(parent_index) && parent_index != 0)//parent bir noktada sağ çocuk oldu//parentın parentını taşımaya çalışıyorum//parent_index % 2 == 0 && parent_index != 0
                            {
                                int parent_parent_index = Parent(parent_index);//parentın parentı, bunu taşımaya çalışıyorum
                                //parentın parentının tablodaki değerini bul

                                int index = heapArray[parent_parent_index];//parenın parentındaki adres değer
                                int parent_key = records[index];//parent parent asıl key
                                int step_size = parent_key / size;//parent adresin üzerine step size eklenmeli
                                //kedni paretnımın üzerine stepsize eklenmeli
                                int current_parent_index = Parent(currentSize);//
                                loc = (heapArray[current_parent_index] + step_size) % size;//bu adres değeri yeni blunduğum currentsize indexine yazılmalı
                                heapArray[currentSize] = loc;//tablodan kontrol edeceğim yeni adres buraası
                                                                       //artık bulunduğum yer dolu mu boş mu kontrol etmeliyim
                            }
                            if (parent_index == 0)//parent index 0 olduysa sol-sol-sol-sol diye giden zincir
                            {
                                int current_parent_index = Parent(currentSize);//
                                int step_size = key / size;
                                loc = (heapArray[current_parent_index] + step_size) % size;//
                                heapArray[currentSize] = loc;
                                //artık bulunduğum yer dolu mu boş mu kontrol etmeliyim

                            }
                            //bulunulan currentsize dolu mu boş mu kontrol et
                        }
                        else//sağ çocuk,//parentı taşımaya çalışıyorum
                        {
                            int parent_index = Parent(currentSize);//bulunduğum yerin parent indexi
                            int index = heapArray[parent_index];//parentın adres değeri
                            int parent_key = records[index];//parentın adres değerindeki asıl key
                            int step_size = parent_key / size;
                            //parent adresin üzerine step size eklenmeli
                            loc = (index + step_size) % size;//loc recırd tablosu üzerindeki indexleri yani adresleri tutar
                            heapArray[currentSize] = loc;//tablodan kontrol edeceğim yeni adres burası

                            //dolu mu boş mu kontrolü lazım
                        }
                    }
                    //whiledan çıktı, boş yer buldu demek oluyor//artık yerleştirme için yeni fonksiyonlar çağırmalıyım
                    if (records[loc] == 0)//boş yer bulundu//loc tablo üzerindeki adres değerini taşır
                    {
                        while (currentSize != 0)
                        {
                            //currentsize sağ çocuk mu sol çocuk mu
                            if (currentSize % 2 == 0)//sağ çocuk
                            { 
                                while (currentSize != 0 && currentSize % 2 == 0)//0.indexe gelene kadar veya sol çocuk olana kadar
                                {
                                    //parentdaki değeri bul ve bulunduğun yere taşı, yeni lokasyonu parent ilan et
                                    int parent_index = Parent(currentSize);//bulunduğum yerin parent indexi
                                    int index = heapArray[parent_index];//parentın adres değeri
                                    int parent_key = records[index];//parentın adres değerindeki asıl key
                                                                    //taşıma işlemi yap(tablo üzerinde)
                                    records[loc] = parent_key;//artık parent boş, yeni index, yeni current size parent olmalı
                                    records[index] = 0;
                                    loc = index;
                                    currentSize = parent_index;
                                }
                                if (currentSize == 0)//artık 0. index, keyi buraya yerleştir
                                {
                                    int index = heapArray[currentSize];// adres değer
                                    records[index] = key;
                                }
                            }
                            else//currentsize sol çocuk//parent sağ çocuk olana kadar yukarı arama yap, ya sağ çocuk olacak ya da 0.index
                            {
                                int parent_index = Parent(currentSize);//parentın indexi
                                while (!(isRightChild(parent_index)))//parent sağ çocuk olana kadar yukarı
                                {
                                    parent_index = Parent(parent_index);
                                }

                                if (parent_index != 0)//parentın parentını bulunduğum adrese taşı, yeni lokasyonu parentın parentı yap
                                {
                                    int parent_parent_index = Parent(parent_index);//parentın parentı, bunu taşımaya çalışıyorum
                                                                                        //parentın parentının tablodaki değerini bul
                                    int index = heapArray[parent_parent_index];//parenın parentındaki adres değer
                                    int parent_key = records[index];//parent parent asıl key

                                    records[loc] = parent_key;//parent parent artık boşaldı, yeni lokasyonum oras
                                    records[index] = 0;
                                    loc = index;
                                    currentSize = parent_parent_index;
                                }
                                else //(parent_index == 0)//parent index 0 olduysa sol-sol-sol-sol diye giden zincir//key bulunduğum locasyona yerleştirmeliyim
                                {
                                    records[loc] = key;
                                    currentSize = 0;
                                }
                            }
                        }
                    }//yerleştirme bitti
                }
            }
            probes = findProbe(keysArray, records);
            Console.WriteLine("\nBINARY TREE METHOD\n");
            Console.WriteLine("{0}\t{1,-10}\t\t{2,-10}", "i", "Key", "Probe");
            for (int i = 0; i < records.Length; i++)
            {
                if(records[i] == 0)
                    Console.WriteLine("{0}\t{1,-10}\t\t{2,-10}", i, "Null", probes[i]);
                else
                    Console.WriteLine("{0}\t{1,-10}\t\t{2,-10}", i, records[i], probes[i]);
            }
        }
        public int[] findProbe(int[] dizi, int[] records)//recordsın içinde dizideki keyleri arayacağım//onu da yeni bir dizide tutacağım
        {
            int[] probes = new int[records.Length];
            int loc;
            int step_size;
            int size = records.Length;
            bool c = false;
            for (int i = 0; i < dizi.Length; i++)
            {
                c = false;
                loc = dizi[i] % size;
                step_size = dizi[i] / size;
                int k;
                for (k = 0; k < records.Length; k++)
                {
                    if (records[loc] == dizi[i])
                    {
                        c = true;
                        break;
                    }
                        
                    else
                    {
                        loc = (loc + step_size) % size;
                    }
                }
                if(c == true)
                    probes[loc] = k + 1;

            }
            return probes;
        }
    }   
    class Program
    {
        static void Main(string[] args)
        {

            //int[] dizi = new int[10] { 27, 18, 29, 28, 39, 13, 16, 41, 17, 19 };
            Bidirectional bidirectional = new Bidirectional();
            Heap heap = new Heap();
            Console.WriteLine("What is the number of keys?(U can change the table size manually in line 364)");
            int keyNumber = Convert.ToInt32(Console.ReadLine());
            int size = 11;//table size manual olarak değiştirilebilir
            int[] dizi = createKeyArray(keyNumber, size);
            bidirectional.createBidirectional(dizi, size);
            heap.createBinary(dizi, size);

            double packingFactor = (double)keyNumber / size;
            packingFactor *= 100;
            Console.WriteLine("Packing Factor --> %"+ packingFactor);

            Console.WriteLine("For BEISCH average probe -->"+ averageProbe(bidirectional.probe, dizi.Length));
            
            Console.WriteLine("For Binary Tree Method average probe -->"+ averageProbe(heap.probes, dizi.Length));

            Console.WriteLine("Which key do you want to search?");
            int searchKey = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("BEISCH");
            int loc1 = searchKeyForBidirect(searchKey, bidirectional.records);
            if (loc1 == -1)
            {
                Console.WriteLine("Not exist");
            }
            else
            {
                Console.WriteLine("{0} key is in index {1}", searchKey, loc1);
            }
          
            Console.WriteLine("BINARY TREE");
            int loc2 = searchKeyForBinary(searchKey, heap.records);
            if (loc2 == -1)
            {
                Console.WriteLine("Not exist");
            }
            else
            {
                Console.WriteLine("{0} key is in index {1}", searchKey, loc2);
            }

        }
        static int searchKeyForBidirect(int key, Node[] records)//linkler takip edilerek probe hesaplanır
        {
            bool c = false;
            int index;
            int size = records.Length;
            int loc = key % size;
            Node node = records[loc];
            while (node != null)
            {
                if (node.data == key)
                {
                    c = true;
                    break;
                }
                if (node.next != null)
                    node = node.next;
                else
                    node = null;
            }
            if (c == true)
            {
                index = Array.IndexOf<Node>(records, node);
                return index;
            }
            else
                return -1;
                
        }
        static int searchKeyForBinary(int key, int[] records)//recordsın içinde dizideki keyleri arayacağım//onu da yeni bir dizide tutacağım
        {
            int loc = -1;
            int size = records.Length;
            int step_size = key / size;
            loc = key % size;
            int i;
            for (i = 0; i < records.Length; i++)
            {
                if (key == records[loc])
                    break;

                else
                {
                    loc = (loc + step_size) % size;
                }

            }
            if (i == records.Length)//bulunamamış
                return -1;
            else
                return loc;

        }
        static float averageProbe(int[] probe, int numberOfKeys)
        {
            float average;
            float toplam = 0;
            for (int i = 0; i < probe.Length; i++)
            {
                toplam += probe[i];
            }
            Console.WriteLine("Total number of probes = "+toplam);
            average = toplam / numberOfKeys;
            return average;
        }

        static int[] createKeyArray(int numberOfKeys, int size)
        {
            Random rnd = new Random();
            Console.WriteLine("Table Size:" + size);
            Console.WriteLine("Number of keys:" + numberOfKeys);
            int[] dizi = new int[numberOfKeys];
            int sayi;
            //random birbirinden farklı sayılar üretilir
            for (int i = 0; i < dizi.Length; i++)
            {
                sayi = rnd.Next(1,int.MaxValue);
                if (Array.IndexOf(dizi, sayi) == -1)//dizide o sayı yok
                    dizi[i] = sayi; 
                else
                    i--;
            }
            Console.WriteLine("\nKEYS\n");
            for (int i = 0; i < dizi.Length; i++)
            {
                Console.WriteLine("{0}\t{1}",i,dizi[i]);
            }
            //Console.Write(" }");
            Console.WriteLine();
            Console.WriteLine();
            return dizi;
        }
    }
}

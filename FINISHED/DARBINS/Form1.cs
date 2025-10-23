using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace DARBINS
{
    public partial class Form1 : Form
    {
        public bool Cilveka_gajiens = false;
        public bool Alfa_beta = false;
        public int AlgDzilums = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void Exit(object sender, EventArgs e)
        {
            Close();
        }
        private void CHANGE(object sender, EventArgs e)
        {   //izlemj vai iet cilvēks vai dators

            if (Cilveka_gajiens)
            {
                try
                {
                    string userInput = Indeks.Text;
                    string[] numbers = userInput.Split(';');
                    int num1 = Convert.ToInt32(numbers[0]);
                    int num2 = Convert.ToInt32(numbers[1]);
                    CilvekaCheckAdjacent(num1, num2);
                }
                catch
                {
                    MessageBox.Show("Notikusi kļūda ievadītājā skaitļu pāru indeksu virknē!",
                    "Kļūda", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
            else
            { // datora gājiens
                List<SpelesKoks> SaknesLimenis = new List<SpelesKoks>();
                SpelesKoks Sakne = new SpelesKoks();
                Sakne.SpelesStavoklis = Result.Text;
                Sakne.Limenis = 0;
                Sakne.CilvekaPunkti = Convert.ToInt32(SpeletejaPunkti.Text);
                Sakne.DatoraPunkti = Convert.ToInt32(DatoraPunkti.Text);
                SaknesLimenis.Add(Sakne);
                
                //automātiska dziļuma regulēšana 
                AlgDzilums = 0;
                int StavoklaGarums = Result.Text.Length;
                int MezgluDaudzums = 1;
                for (;MezgluDaudzums < 255024 && StavoklaGarums != 1; AlgDzilums++) 
                {
                    StavoklaGarums -= 1;
                    MezgluDaudzums *= StavoklaGarums;
                }
                --AlgDzilums;
                //uzkonstruē koku un izvēlas kuru algoritmu izmantot lai noteiktu svarus
                List<SpelesKoks> Strupceli = KonstrueSpelesKoku(SaknesLimenis, 0);

                if (Alfa_beta)
                    AlfaBeta(Strupceli[0]);
                else
                    Minimax(Strupceli);

                // izvēlās gājienu
                SpelesKoks DatoraGajiens = Sakne.Berni[0];
                foreach (var Izvele in Sakne.Berni)
                {
                    if (DatoraGajiens.Svars < Izvele.Svars)
                        DatoraGajiens = Izvele;
                }
                //ievada informāciju gui
                SpeletejaPunkti.Text = DatoraGajiens.CilvekaPunkti.ToString();
                DatoraPunkti.Text = DatoraGajiens.DatoraPunkti.ToString();
                Result.Text = DatoraGajiens.SpelesStavoklis;
                //pārslēdz mainīgos uz cilvēka gājienu
                Cilveka_gajiens = true;
                Indeks.ReadOnly = false;
                button1.Text = "Cilvēka gājiens";
            }
            //nosaka uzvarētajus
            if (Result.Text.Length == 1)
            {
                button1.Enabled = false;
                if (Convert.ToInt32(DatoraPunkti.Text) > Convert.ToInt32(SpeletejaPunkti.Text))
                    Result.Text = "Uzvar dators!";
                else if (Convert.ToInt32(DatoraPunkti.Text) < Convert.ToInt32(SpeletejaPunkti.Text))
                    Result.Text = "Uzvar cilvēks!";
                else
                    Result.Text = "Neizšķirts!";
            }

        }
        private List<SpelesKoks> KonstrueSpelesKoku(List<SpelesKoks> VecakuLimenis, int limenis) 
        {
            //saņem listu ar vecākiem un līmeni
            //uztaisa listu ar bērniem kur glabā visus mezglus šajā līmenī
            List<SpelesKoks> BernuLimenis = new List<SpelesKoks>();
            limenis += 1;
            //izvēlas katru vecāku un vecāku līmeņa
            foreach (var mezgls in VecakuLimenis) 
            {
                string stavoklis = mezgls.SpelesStavoklis;
                //cikls kas generē bērnus visiem iespējamajiem gājieniem
                for (int i = 0; i < stavoklis.Length - 1; i++)
                {
                    SpelesKoks berns = new SpelesKoks();
                    berns.Limenis = limenis;
                    berns.DatoraPunkti = mezgls.DatoraPunkti;
                    berns.CilvekaPunkti = mezgls.CilvekaPunkti;                 
                    string pair = stavoklis.Substring(i, 2);
                    //katra iespējamā gājiena datu ievade bērnā
                    if (pair == "00")
                    {
                        berns.SpelesStavoklis = stavoklis.Substring(0, i) + "1";
                        if (i + 2 < stavoklis.Length)
                            berns.SpelesStavoklis = berns.SpelesStavoklis + stavoklis.Substring(i + 2);
                        if (berns.Limenis % 2 != 0)
                            berns.DatoraPunkti += 1;
                        else
                            berns.CilvekaPunkti += 1;
                    }
                    else if (pair == "01")
                    {
                        berns.SpelesStavoklis = stavoklis.Remove(i + 1, 1);
                        if (berns.Limenis % 2 != 0)
                            berns.CilvekaPunkti += 1;
                        else
                            berns.DatoraPunkti += 1;
                    }
                    else if (pair == "10")
                    {
                        berns.SpelesStavoklis = stavoklis.Remove(i + 1, 1);
                        if (berns.Limenis % 2 != 0)
                            berns.CilvekaPunkti -= 1;
                        else
                            berns.DatoraPunkti -= 1;
                    }
                    else if (pair == "11")
                    {
                        berns.SpelesStavoklis = stavoklis.Substring(0, i) + "0";
                        if (i + 2 < stavoklis.Length)
                            berns.SpelesStavoklis = berns.SpelesStavoklis + stavoklis.Substring(i + 2);
                        if (berns.Limenis % 2 != 0)
                            berns.DatoraPunkti += 1;
                        else
                            berns.CilvekaPunkti += 1;
                    }
                    //meklējam dublikātus esošā līmenī
                    foreach (var dvinis in BernuLimenis)
                    {
                        if (dvinis.SpelesStavoklis == berns.SpelesStavoklis &&
                            dvinis.DatoraPunkti == berns.DatoraPunkti &&
                            dvinis.CilvekaPunkti == berns.CilvekaPunkti)
                        {
                            if (!dvinis.Vecaki.Contains(mezgls))
                                dvinis.PievienoVecaku(mezgls);
                            if (!mezgls.Berni.Contains(dvinis))
                                mezgls.PievienoBernu(dvinis);
                            berns = null;
                            break;
                        }                        
                    }
                    //ja nav dublikātu, pievieno esošam līmenim bērnu
                    if (berns != null)
                    {
                        mezgls.PievienoBernu(berns);
                        berns.PievienoVecaku(mezgls);
                        BernuLimenis.Add(berns);
                    }
                }
            }
            // ja sasniegts dziļuma vai iespējamo gājienu limits, pārtrauc ģenerēt
            if (limenis >= AlgDzilums || BernuLimenis[0].SpelesStavoklis.Length < 2)
            {
                foreach (var berns in BernuLimenis)
                {
                    berns.Svars = berns.DatoraPunkti - berns.CilvekaPunkti;
                    berns.NavPagaiduIrSvars = 2;
                }
                textBox1.Text = limenis.ToString();
                return BernuLimenis;
            }
            //ja limiti nav sasniegti, sūta esošo bērnu līmeni kā vecākus rekursīvi uz sevi pašu
            else
            {                
                return KonstrueSpelesKoku(BernuLimenis, limenis);
                
            }
        }
        //Minmax piešķir vērtības pa līmeņiem no apakšas uz augšu
        private void Minimax(List<SpelesKoks> Strupceli) 
        {
            //saņem listu ar koka strupceļa virsotnēm
            //ģenerē priekšteču līmeni
            List <SpelesKoks> NakamaisLimenis = new List<SpelesKoks>();
            //izvēlas katru strupceļa virsotni...
            foreach (var mezgls in Strupceli)
            {
                //...tai izvēlas katru vecāku..
                foreach (var vecaks in mezgls.Vecaki)
                {
                    //..ja vecākam ir svars, tad to palaiž garām..
                    if (vecaks.NavPagaiduIrSvars == 2)
                        continue;
                    //..ja svara nav un vairāki bērni tad izvēlas vai nu mazāko vai lielāko..
                    if (vecaks.Berni.Count > 1)
                    {
                        int svars;
                        // ..max vai min līmeni nosaka ar līmeņa atlikuma palīdzību..
                        if (vecaks.Limenis % 2 != 0)
                        {
                            svars = int.MinValue;
                            foreach (var berns in vecaks.Berni)
                            {
                                if (berns.Svars > svars)
                                    svars = berns.Svars;
                            }
                        }
                        else
                        {
                            svars = int.MaxValue;
                            foreach (var berns in vecaks.Berni)
                            {
                                if (berns.Svars < svars)
                                    svars = berns.Svars;
                            }
                        }
                        //..NavPagaiduIrSvars vērtība 2 norāda, ka ir noteikts svars..
                        vecaks.NavPagaiduIrSvars = 2;
                        vecaks.Svars = svars;
                    }
                    //..ja tikai viens bērns, tad ņem viņa svaru
                    else
                    {
                        vecaks.NavPagaiduIrSvars = 2;
                        vecaks.Svars = vecaks.Berni[0].Svars;
                    }
                    //..ja augstāka līmeņa sarakstā jau nav vecāka, to pievieno..
                    if (!NakamaisLimenis.Contains(vecaks))
                        NakamaisLimenis.Add(vecaks);
                }
            }
            //kad visi esošā līmeņa mezglu vecāki apskatīti, šos vecākus sūta kā jaunu apakšējo līmeni
            if (NakamaisLimenis[0].Limenis != 0)
                Minimax(NakamaisLimenis);
        }
        private void AlfaBeta(SpelesKoks Strupcels)
        {//saņem pirmo strupceļa mezglu..
            //..ja tam ir vairāki bērni tad..
            if (Strupcels.Vecaki[0].Berni.Count > 1)
            {
                //..nosaka max vai min līmenis..
                if (Strupcels.Vecaki[0].Limenis % 2 != 0)
                    Strupcels.Vecaki[0].Svars = int.MinValue;
                else Strupcels.Vecaki[0].Svars = int.MaxValue;
                //..tad katram vecāka bērnam pārbauda vai ir noteikts svars..
                foreach (var berns in Strupcels.Vecaki[0].Berni)
                {
                    //..ja svara nav, tad šo bērnu sūta uz AlfaBetaIedzilinas, kas tai atrod svaru
                    if (berns.NavPagaiduIrSvars == 0)
                    {
                        AlfaBetaIedzilinas(Strupcels.Vecaki[0], berns);
                    }
                    //..atkarībā no līmeņa ņem vai nu mazāko vai lielāko svaru
                    if (Strupcels.Vecaki[0].Limenis % 2 != 0 && Strupcels.Vecaki[0].Svars < berns.Svars) //meklee max
                    {
                        Strupcels.Vecaki[0].Svars = berns.Svars;
                    }
                    else if (Strupcels.Vecaki[0].Limenis % 2 == 0 && Strupcels.Vecaki[0].Svars > berns.Svars) //meklee min
                    {
                        Strupcels.Vecaki[0].Svars = berns.Svars;
                    }

                }
            }
            //..ja bērns ir viens, tad vienkārši paņem tā svaru
            else
            {
                Strupcels.Vecaki[0].Svars = Strupcels.Svars;
                Strupcels.Vecaki[0].NavPagaiduIrSvars = 2;
            }
            //ja strupceļa virsotne ir pirmspēdējā,tad tās vecāks ir pēdējais, kas nozīmē, ka ceļš atrasts
            if (Strupcels.Limenis != 1)
                AlfaBeta(Strupcels.Vecaki[0]);
        }
        private void AlfaBetaIedzilinas(SpelesKoks vecvecaks, SpelesKoks vecaks)
        {//saņem mezglu "vecāks" kam jānosaka svars iedziļinoties un tā vecāku "vecvecāks"            
            bool navBernuSvaru = true; // karogs, ja vecākam nevienam bērnam nav svaru
            vecaks.NavPagaiduIrSvars = 2;
            int svars;
            //nosaka ko meklē, max vai min vērtību
            if (vecaks.Limenis % 2 != 0)
                svars = int.MinValue;
            else svars = int.MaxValue;
            //nosaka vai vecakam ir berns ar svaru, ja jaa, piešķir vērtību
            foreach (var berns in vecaks.Berni)
            {
                //ja ir vismaz viens bērns bez svara, tad vecāka svars ir pagaidu
                if (berns.NavPagaiduIrSvars == 0)
                    vecaks.NavPagaiduIrSvars = 1; //pagaidu svara karogs
                else if (berns.NavPagaiduIrSvars == 2)
                {
                    //ja kaut vienam bernam svars ir tad vecakam ir pagaidu svars
                    navBernuSvaru = false;
                    if (vecaks.Limenis % 2 != 0 && svars < berns.Svars) //meklee max
                    {
                        svars = berns.Svars;
                    }
                    else if (vecaks.Limenis % 2 == 0 && svars > berns.Svars) //meklee min
                    {
                        svars = berns.Svars;
                    }
                }
            }
            //ja ne vienam bernam svara nebija, tad vecakam arī svara nav
            if (navBernuSvaru == true) 
            {
                vecaks.NavPagaiduIrSvars = 0;
            }
            else vecaks.Svars = svars;
            //ja vecaka visiem bērniem ir svari, tad vecaka svars ir noteikts
            if (vecaks.NavPagaiduIrSvars == 2)
                return;
            //ja vecvecākam nav pat pagaidu svara, tad..
            if (vecvecaks.NavPagaiduIrSvars == 0) 
            {
                //..vecākam nosaka svaru neņemot verā konfliktus ar vecvecāku
                foreach (var berns in vecaks.Berni)
                {
                    if (berns.NavPagaiduIrSvars == 0)
                        AlfaBetaIedzilinas(vecaks, berns);
                }
                if (vecaks.Limenis % 2 != 0)
                    svars = int.MinValue;
                else svars = int.MaxValue;
                foreach (var berns in vecaks.Berni)
                {
                    if (vecaks.Limenis % 2 != 0 && svars < berns.Svars) //meklee max
                    {
                        svars = berns.Svars;
                    }
                    else if (vecaks.Limenis % 2 == 0 && svars > berns.Svars) //meklee min
                    {
                        svars = berns.Svars;
                    }
                }
                vecaks.Svars = svars;
                vecaks.NavPagaiduIrSvars = 2;
                return;
            }
            //ja vecvecākam ir pagaidu svars tad..
            else if (vecvecaks.NavPagaiduIrSvars == 1)
            {
                //..ja vecākam nav pat pagaidu svara..
                if (vecaks.NavPagaiduIrSvars == 0)
                {
                    //..ir jāiedziļinās vēl dziļāk un jānosaka tam pagaidu svars
                    AlfaBetaIedzilinas(vecaks, vecaks.Berni[0]);
                    vecaks.Svars = vecaks.Berni[0].Svars;
                    vecaks.NavPagaiduIrSvars = 1;
                }
                //tagad vecakam ir pagaidu svars, var meklēt konfliktus ar vecvecāku
                foreach (var berns in vecaks.Berni)
                {
                    //veic zaru griešanu
                    if (vecaks.Limenis % 2 != 0 && vecaks.Svars <= vecvecaks.Svars)
                    {
                        foreach (var bernsIn in vecaks.Berni)
                        {
                            if (bernsIn.NavPagaiduIrSvars == 0)
                            {
                                bernsIn.Svars = int.MinValue;
                                bernsIn.NavPagaiduIrSvars = 2;
                            }
                        }
                        vecaks.NavPagaiduIrSvars = 2;
                        return;
                    }
                    else if (vecaks.Limenis % 2 == 0 && vecaks.Svars >= vecvecaks.Svars)
                    {
                        foreach (var bernsIn in vecaks.Berni)
                        {
                            if (bernsIn.NavPagaiduIrSvars == 0)
                            {
                                bernsIn.Svars = int.MaxValue;
                                bernsIn.NavPagaiduIrSvars = 2;
                            }
                        }
                        vecaks.NavPagaiduIrSvars = 2;
                        return;
                    }
                    //ja griešanu neveic, tad aprēķina nākamam bērnam svaru
                    if (berns.NavPagaiduIrSvars == 0)
                        AlfaBetaIedzilinas(vecaks, berns);
                }
                //ja griešanas nebija, piešķir vērtību max vai min
                if (vecaks.Limenis % 2 != 0)
                    svars = int.MinValue;
                else svars = int.MaxValue;
                foreach (var berns in vecaks.Berni)
                {
                    if (vecaks.Limenis % 2 != 0 && svars < berns.Svars) //meklee max
                    {
                        svars = berns.Svars;
                    }
                    else if (vecaks.Limenis % 2 == 0 && svars > berns.Svars) //meklee min
                    {
                        svars = berns.Svars;
                    }
                }
                vecaks.NavPagaiduIrSvars = 2;
            }    
        }
        private void CilvekaCheckAdjacent(int num1, int num2)
        {
            if (Math.Abs(num1 - num2) == 1)
            {
                CilvekaGajiens(num1, num2);
            }
            else
            {
                MessageBox.Show("Ievadītie indeksi nav blakusesošie",
                "Kļūda", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }
        private void CilvekaGajiens(int num1, int num2)
        {
            string pair = Result.Text.Substring(num1, 2); // Get the pair of numbers at the adjacent indices

            if (pair == "00")
            {
                SpeletejaPunkti.Text = (Convert.ToInt32(SpeletejaPunkti.Text) + 1).ToString(); // Add 1 to the player's score
                //Result.Text = Result.Text.Remove(num1, 2);  Remove the pair from the sequence
                string dala = "";
                if (num1 + 2 < Result.Text.Length)
                {
                    dala = Result.Text.Substring(num1 + 2);
                }                    
                Result.Text = Result.Text.Substring(0, num1) + "1" + dala;
            }
            else if (pair == "01")
            {
                DatoraPunkti.Text = (Convert.ToInt32(DatoraPunkti.Text) + 1).ToString(); // Add 1 to the opponent's score
                Result.Text = Result.Text.Remove(num1 + 1, 1); // Remove the pair from the sequence
            }
            else if (pair == "10")
            {
                //SpeletejaPunkti.Text = (Convert.ToInt32(SpeletejaPunkti.Text) + 1).ToString(); // Add 1 to the player's score
                DatoraPunkti.Text = (Convert.ToInt32(DatoraPunkti.Text) - 1).ToString(); // Subtract 1 from the opponent's score
                Result.Text = Result.Text.Remove(num1 + 1, 1); // Remove the pair from the sequence
            }
            else if (pair == "11")
            {
                SpeletejaPunkti.Text = (Convert.ToInt32(SpeletejaPunkti.Text) + 1).ToString(); // Add 1 to the player's score
                //Result.Text = Result.Text.Remove(num1,2);  Remove the pair from the sequence
                string dala = "";
                if (num1 + 2 < Result.Text.Length)
                {
                    dala = Result.Text.Substring(num1 + 2);
                }
                Result.Text = Result.Text.Substring(0, num1) + "0" + dala;
            }

            Cilveka_gajiens = false;
            Indeks.ReadOnly = true;
            button1.Text = "Datora gājiens";
            Indeks.Text = "";
        }
        private void ENTER(object sender, EventArgs e)
        {
            try
            {
                if (!izvele_1.Checked && !izvele_2.Checked)
                {
                    MessageBox.Show("Netika izvēlēts, kurš uzsāk spēli!",
                    "Kļūda", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
                }
                if (!checkBox1.Checked && !checkBox2.Checked)
                {
                    MessageBox.Show("Netika izvēlēts, kuru algoritmu izmantot!",
                    "Kļūda", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    return;
                }

                int size = Convert.ToInt32(SkaitluVirkne.Text);
                SpeletejaPunkti.Text = "0";
                DatoraPunkti.Text = "0";

                if (size >= 15 && size <= 25)
                {
                    GenerateArrow(size);
                }
                else { 
                    MessageBox.Show("Netika ievadīts nepieciešamais skaitļu virknes garums!",
                    "Kļūda", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

            }
            catch
            {
                MessageBox.Show("Notikusi kļūda ievadītājā virknes skaitļu garumā!",
                "Kļūda", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }
        private void GenerateArrow(int size)
        {
            izvele_1.Enabled = false;
            izvele_2.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;

            Random random = new Random();

            string arrow = "";

            for (int i = 0; i < size; i++)
            {
                int direction = random.Next(0, 2); // Generates 0 or 1
                arrow += direction;
            }

            if (izvele_1.Checked)
            {
                button1.Text = "Cilvēka gājiens";
                Cilveka_gajiens = true;
            }
            else
            {
                button1.Text = "Datora gājiens";
                Indeks.ReadOnly = true;
            }
            if (checkBox1.Checked)
            {
                Alfa_beta = true;
            }
            button1.Enabled = true;
            //textBox1.ReadOnly = true;

            SkaitluVirkne.ReadOnly = true;
            Result.Text = arrow;
        }
        private void RESTART(object sender, EventArgs e)
        {
            Application.Restart();

        }

        private void izvele_1_CheckedChanged(object sender, EventArgs e)
        {
            if (izvele_1.Checked)
            {
                izvele_2.Checked = false;
            }
        }

        private void izvele_2_CheckedChanged(object sender, EventArgs e)
        {
            if (izvele_2.Checked)
            {
                izvele_1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }
    }
}

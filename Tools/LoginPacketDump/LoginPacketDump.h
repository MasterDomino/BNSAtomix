#ifdef LOGINPACKETDUMP_EXPORTS
#define LOGINPACKETDUMP_API __declspec(dllexport)
#else
#define LOGINPACKETDUMP_API __declspec(dllimport)
#endif
class LOGINPACKETDUMP_API CLoginPacketDump
{
public:
    CLoginPacketDump(void);
};

struct BlockBuffer
{
public:
    char* ptr;
    int size;
    int u1;
    int u2;
};

struct Trunc
{
    int size;
    char* buffer;
};


namespace SagaBNS
{
    namespace LoginPacketDump
    {

        public ref class Dumper sealed abstract
        {
            static bool setup = false;
            static System::Threading::Thread^ keyThread;
            static int* key;
            static int lastKey;
            static System::String^ keyFileName;//
            static System::String^ packetFileName;

        public:

            ///<summary>Gets number of CPU ticks clocked since CPU is started</summary>
            static void DumpSend(BlockBuffer* buffer, unsigned __int32 truncCount, Trunc* tuncs, int totalbytes, int* gameKey)
            {
                /*if(!setup)
                {
                    setup=true;
                    key=gameKey;
                    keyThread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(DumpKey));
                    keyThread->Start();
                }*/
                array<unsigned char> ^buf = gcnew array<unsigned char>(totalbytes);
                pin_ptr<unsigned char> dst = &buf[0];
                int index = 0;
                //DumpKey(gameKey);
                for (int i = 0; i < truncCount; i++)
                {
                    memcpy(&dst[index], tuncs[i].buffer, tuncs[i].size);
                    index += tuncs[i].size;
                }
                try
                {
                    MakeSurePacketFile();
                    System::IO::StreamWriter ^sw = gcnew System::IO::StreamWriter(packetFileName, true, System::Text::Encoding::UTF8);
                    sw->WriteLine(System::String::Format("[Send][{0}] {1}", System::DateTime::Now, System::Text::Encoding::UTF8->GetString(buf)));
                    sw->Close();
                }
                catch (System::Exception ^ex)
                {
                }
            }

            static void MakeSurePacketFile()
            {
                if (!packetFileName)
                {
                    if (!System::IO::Directory::Exists("Log"))
                        System::IO::Directory::CreateDirectory("Log");
                    System::DateTime^ now = System::DateTime::Now;
                    packetFileName = System::String::Format("Log\\PacketLog_{0}_{1}_{2}_{3}_{4}_{5}.txt", now->Year, now->Month, now->Day, now->Hour, now->Minute, now->Second);
                }
            }
            static void MakeSureKeyFile()
            {
                if (!keyFileName)
                {
                    if (!System::IO::Directory::Exists("Log"))
                        System::IO::Directory::CreateDirectory("Log");
                    System::DateTime^ now = System::DateTime::Now;
                    keyFileName = System::String::Format("Log\\KeyLog_{0}_{1}_{2}_{3}_{4}_{5}.txt", now->Year, now->Month, now->Day, now->Hour, now->Minute, now->Second);
                }
            }
            static void DumpEncKey(char* key, int length)
            {
                if (length == 0x10)
                {
                    array<unsigned char> ^buf = gcnew array<unsigned char>(length);
                    pin_ptr<unsigned char> dst = &buf[0];
                    memcpy(dst, key, length);
                    try
                    {
                        MakeSureKeyFile();
                        System::IO::StreamWriter ^sw = gcnew System::IO::StreamWriter(keyFileName, true, System::Text::Encoding::UTF8);
                        sw->WriteLine(System::String::Format("[Got Key][{0}] {1}", System::DateTime::Now, bytes2HexString(buf)));
                        sw->Close();
                    }
                    catch (System::Exception ^ex)
                    {
                    }
                }
            }
            static System::String^ bytes2HexString(array<unsigned char>^ b)
            {
                System::String^ tmp = gcnew System::String(L"");
                int i;
                for (i = 0; i < b->Length; i++)
                {
                    System::String^ tmp2 = b[i].ToString("X2");
                    tmp = tmp + tmp2;
                }
                return tmp;
            }
            static void DumpReceive(char* buffer, int length)
            {
                array<unsigned char> ^buf = gcnew array<unsigned char>(length);
                pin_ptr<unsigned char> dst = &buf[0];
                memcpy(dst, buffer, length);
                try
                {
                    System::String^ content = System::Text::Encoding::UTF8->GetString(buf);
                    if (content->Contains("<Reply>\n<Token>"))
                    {
                        MakeSureKeyFile();
                        System::IO::StreamWriter ^sw2 = gcnew System::IO::StreamWriter(keyFileName, true, System::Text::Encoding::UTF8);
                        sw2->WriteLine(System::String::Format("[Got Session Token][{0}]", System::DateTime::Now));
                        sw2->Close();
                    }

                    MakeSurePacketFile();
                    System::IO::StreamWriter ^sw = gcnew System::IO::StreamWriter(packetFileName, true, System::Text::Encoding::UTF8);
                    sw->WriteLine(System::String::Format("[Recieve][{0}] {1}", System::DateTime::Now, content));
                    sw->Close();
                }
                catch (System::Exception ^ex)
                {
                }
            }
        };
    }
}

LOGINPACKETDUMP_API void DumpPacketSend(BlockBuffer* buffer, unsigned __int32 truncCount, void* tuncs, int totalbytes);

LOGINPACKETDUMP_API void DumpPacketReceive(char* buffer, int length);

void DumpPacketSendIntern(BlockBuffer* buffer, unsigned __int32 truncCount, Trunc* tuncs, int totalbytes);
void DumpPacketReceiveIntern(char* buffer, int length);

void DumpEncKey(void* unknown, char* key, int length);
void DumpEncKeyIntern(char* key, int length);


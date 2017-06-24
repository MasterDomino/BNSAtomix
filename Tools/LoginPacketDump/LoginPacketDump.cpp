#include "stdafx.h"
#include "LoginPacketDump.h"


LOGINPACKETDUMP_API void DumpPacketSend(BlockBuffer* buffer,unsigned __int32 truncCount,Trunc* tuncs,int totalbytes)
{	
	__asm
	{
		push ecx
		mov eax,totalbytes 
		push eax
		mov eax,tuncs
		push eax
		mov eax, truncCount
		push eax
		mov eax, buffer
		push eax
		call DumpPacketSendIntern
		add esp,0x10
		pop ecx
	}
}

bool setup;
LOGINPACKETDUMP_API void DumpPacketReceive(char* buffer,int length)
{
	__asm push ecx;
	DumpPacketReceiveIntern(buffer,length);
	__asm pop ecx;
}

const int AES_CREATE_KEY_CALLBACK_ADDR_CBT2=0x73A520;
const int AES_CREATE_KEY_CALLBACK_ADDR_CBT3=0x9235BC;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV80=0x91D43C;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV81=0x9235BC;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV82=0x92968C;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV97=0x989518;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV109=0x9747DC;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV112=0x9833EC;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV114=0x99C1AC;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV116=0x9AD70C;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV119=0x9B082C;
const int AES_CREATE_KEY_CALLBACK_ADDR_RETAILV127=0x9CC224;
const int AES_CREATE_KEY_ENTRY_CBT2=0x5DEA20;
const int AES_CREATE_KEY_ENTRY_CBT3=0x714E80;
const int AES_CREATE_KEY_ENTRY_RETAILV80=0x710650;
const int AES_CREATE_KEY_ENTRY_RETAILV81=0x714E80;
const int AES_CREATE_KEY_ENTRY_RETAILV82=0x719290;
const int AES_CREATE_KEY_ENTRY_RETAILV97=0x75A670;
const int AES_CREATE_KEY_ENTRY_RETAILV109=0x76A630;
const int AES_CREATE_KEY_ENTRY_RETAILV112=0x777450;
const int AES_CREATE_KEY_ENTRY_RETAILV114=0x7886A0;
const int AES_CREATE_KEY_ENTRY_RETAILV116=0x794AE0;
const int AES_CREATE_KEY_ENTRY_RETAILV119=0x7972C0;
const int AES_CREATE_KEY_ENTRY_RETAILV127=0x7A8260;


void DumpPacketSendIntern(BlockBuffer* buffer,unsigned __int32 truncCount,Trunc* tuncs,int totalbytes)
{
	if(!setup)
	{
		setup=true;
		int addr=AES_CREATE_KEY_CALLBACK_ADDR_RETAILV127;
		char* mem= (char*)addr;
		int ptr=(int)&DumpEncKey;
		*(int*)mem=ptr;
	}
	SagaBNS::LoginPacketDump::Dumper::DumpSend(buffer,truncCount,tuncs,totalbytes,0);	
}

typedef void (createAesKey)(void* unknown, char* key, int length);
void DumpEncKey(void* unknown, char* key, int length)
{
	int addr= AES_CREATE_KEY_ENTRY_RETAILV127;
	int a=length;
	DumpEncKeyIntern(key,length);
	createAesKey* createKey=(createAesKey*)addr;
	createKey(unknown,key,length);
}

void DumpPacketReceiveIntern(char* buffer,int length)
{
	SagaBNS::LoginPacketDump::Dumper::DumpReceive(buffer,length);
}

void DumpEncKeyIntern(char* key,int length)
{
	SagaBNS::LoginPacketDump::Dumper::DumpEncKey(key,length);
}

CLoginPacketDump::CLoginPacketDump()
{
	return;
}

#include "MorseCode.h"


MorseCode::MorseCode(uint16_t unitDurationMsec)
	: unit(unitDurationMsec)
{
}

void MorseCode::Send(char* signal)
{
	auto *letter = signal;

	while (*letter != '/0')
	{
		SendLetter(*letter);


	}
}

void MorseCode::SendLetter(char signal)
{
	
}

MorseCode::~MorseCode()
{
}

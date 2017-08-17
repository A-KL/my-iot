#pragma once
#include <cstdio>
#include <stdint.h>

class MorseCode
{
public:
	MorseCode(uint16_t unitDurationMsec);

	void Send(char* signal);

	void SendLetter(char signal);

	~MorseCode();

private:
	uint16_t unit;

};


#include <cstdio>
#include "bcm2835.h"

#define UNIT_DURATION_MS		100 //ms

#define DOT_DURATION_MS			UNIT_DURATION_MS * 1
#define DASH_DURATION_MS		UNIT_DURATION_MS * 3

#define SAME_LETTER_SPACE_MS	UNIT_DURATION_MS * 1

#define NEW_LETTER_SPACE_MS		UNIT_DURATION_MS * 3
#define NEW_WORD_SPACE_MS		UNIT_DURATION_MS * 7


#define SOS_LIGHT_PIN		RPI_V2_GPIO_P1_21


void Dot()
{
	bcm2835_gpio_write(SOS_LIGHT_PIN, HIGH);
	delay(DOT_DURATION_MS);
	bcm2835_gpio_write(SOS_LIGHT_PIN, LOW);}

void Dash()
{
	bcm2835_gpio_write(SOS_LIGHT_PIN, HIGH);
	delay(DASH_DURATION_MS);
	bcm2835_gpio_write(SOS_LIGHT_PIN, LOW);
}

int main()
{
	printf("hello from SosDemo!\n");

	bcm2835_init();
	bcm2835_gpio_fsel(SOS_LIGHT_PIN, BCM2835_GPIO_FSEL_OUTP);

	printf("Starting SOS sequence...\n");

	// while(true)
	{
		// S
		printf("S(...)");		
		Dot();
		delay(SAME_LETTER_SPACE_MS);
		Dot();
		delay(SAME_LETTER_SPACE_MS);
		Dot();

		// O
		printf("O(---)");
		delay(NEW_LETTER_SPACE_MS);
		Dash();
		delay(SAME_LETTER_SPACE_MS);
		Dash();
		delay(SAME_LETTER_SPACE_MS);
		Dash();

		// S
		printf("S(...)");
		delay(NEW_LETTER_SPACE_MS);
		Dot();
		delay(SAME_LETTER_SPACE_MS);
		Dot();
		delay(SAME_LETTER_SPACE_MS);
		Dot();

		printf("\n");
		delay(NEW_WORD_SPACE_MS);
	}

    return 0;
}
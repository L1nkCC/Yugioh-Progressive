using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
public class CardParticles
{
    ParticleSystem[] particles = new ParticleSystem[Utilities.Enum.Length(typeof(BTN_Option))];

    public CardParticles(Card card)
    {
        particles[(int)BTN_Option.normalSummon]= card.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        particles[(int)BTN_Option.specialSummonAttack] = card.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();
        particles[(int)BTN_Option.specialSummonDefense] = card.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>();
        particles[(int)BTN_Option.activate]= card.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>();
        particles[(int)BTN_Option.attack] = card.transform.GetChild(0).GetChild(3).GetComponent<ParticleSystem>();
    }

    private static void PlayParticles(Card card, BTN_Option option)
    {
        if (card.particles.particles[(int)option] == null)
            return;
        card.particles.particles[(int)option].Play();
    }

    public void AttachToButtons()
    {
        BTN_Handler.BTN_Assign(BTN_Option.specialSummonAttack,((Card card) => { PlayParticles(card, BTN_Option.specialSummonAttack); }));
        BTN_Handler.BTN_Assign(BTN_Option.specialSummonDefense,((Card card) => { PlayParticles(card, BTN_Option.specialSummonDefense); }));
        BTN_Handler.BTN_Assign(BTN_Option.normalSummon,((Card card) => { PlayParticles(card, BTN_Option.normalSummon); }));
        BTN_Handler.BTN_Assign(BTN_Option.activate,((Card card) => { PlayParticles(card, BTN_Option.activate); }));
        BTN_Handler.BTN_Assign(BTN_Option.attack,((Card card) => { PlayParticles(card, BTN_Option.attack); }));
    }
}

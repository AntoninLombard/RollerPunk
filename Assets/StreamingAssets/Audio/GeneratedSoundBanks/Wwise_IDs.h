/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID AMB_BALL_LOST = 3846266530U;
        static const AkUniqueID AMB_DRILL_DONE = 2925030600U;
        static const AkUniqueID AMB_RUNSTART = 2349163407U;
        static const AkUniqueID AMB_START = 1589289402U;
        static const AkUniqueID BALL_GRAB = 2618017047U;
        static const AkUniqueID BALL_PUNCH_MOVE = 155540885U;
        static const AkUniqueID CHECKPOINT = 612075679U;
        static const AkUniqueID COLLISION = 1792046091U;
        static const AkUniqueID COUNTDOWN = 1505888634U;
        static const AkUniqueID COUNTER = 3051750219U;
        static const AkUniqueID ENGINE_START = 2862969430U;
        static const AkUniqueID HIT_BYBALL = 602331051U;
        static const AkUniqueID HIT_BYPUNCH = 3549005828U;
        static const AkUniqueID HIT_BYWALL = 2308453416U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PARRY_MOVE = 1084997411U;
        static const AkUniqueID PARRY_SUCCESS = 1134971709U;
        static const AkUniqueID PLAY_OUTRO = 431014199U;
        static const AkUniqueID PLAY_TEST = 3187507146U;
        static const AkUniqueID PUNCH_AFTERPARRY = 972759482U;
        static const AkUniqueID PUNCH_MOVE = 3055455917U;
        static const AkUniqueID PUNCH_TAUNT = 1403211570U;
        static const AkUniqueID SETPLAYERNUMBER = 3687251531U;
        static const AkUniqueID TEST = 3157003241U;
        static const AkUniqueID UI_BACK = 2024222415U;
        static const AkUniqueID UI_HOVER = 2118900976U;
        static const AkUniqueID UI_SELECT = 2774129122U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace MUSIC
        {
            static const AkUniqueID GROUP = 3991942870U;

            namespace STATE
            {
                static const AkUniqueID BED = 446279778U;
                static const AkUniqueID END = 529726532U;
                static const AkUniqueID MENU = 2607556080U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID P1 = 1635194252U;
                static const AkUniqueID P2 = 1635194255U;
                static const AkUniqueID P3 = 1635194254U;
                static const AkUniqueID P4 = 1635194249U;
            } // namespace STATE
        } // namespace MUSIC

        namespace MUSIC_STATE
        {
            static const AkUniqueID GROUP = 3826569560U;

            namespace STATE
            {
                static const AkUniqueID END = 529726532U;
                static const AkUniqueID GAMEPLAY = 89505537U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace MUSIC_STATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GROUNDED
        {
            static const AkUniqueID GROUP = 2907122923U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace GROUNDED

        namespace PLAYER
        {
            static const AkUniqueID GROUP = 1069431850U;

            namespace SWITCH
            {
                static const AkUniqueID P1 = 1635194252U;
                static const AkUniqueID P2 = 1635194255U;
                static const AkUniqueID P3 = 1635194254U;
                static const AkUniqueID P4 = 1635194249U;
            } // namespace SWITCH
        } // namespace PLAYER

        namespace THROTTLE
        {
            static const AkUniqueID GROUP = 2995819693U;

            namespace SWITCH
            {
                static const AkUniqueID FALSE = 2452206122U;
                static const AkUniqueID TRUE = 3053630529U;
            } // namespace SWITCH
        } // namespace THROTTLE

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID DIRECTION = 16764168U;
        static const AkUniqueID MASTER = 4056684167U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PAUSE = 3092587493U;
        static const AkUniqueID PLAYER_NUMBER = 1659248646U;
        static const AkUniqueID RPM = 796049864U;
        static const AkUniqueID SCORE = 2398231425U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID SKID = 822292696U;
        static const AkUniqueID SPEED = 640949982U;
        static const AkUniqueID SS_AIR_FEAR = 1351367891U;
        static const AkUniqueID SS_AIR_FREEFALL = 3002758120U;
        static const AkUniqueID SS_AIR_FURY = 1029930033U;
        static const AkUniqueID SS_AIR_MONTH = 2648548617U;
        static const AkUniqueID SS_AIR_PRESENCE = 3847924954U;
        static const AkUniqueID SS_AIR_RPM = 822163944U;
        static const AkUniqueID SS_AIR_SIZE = 3074696722U;
        static const AkUniqueID SS_AIR_STORM = 3715662592U;
        static const AkUniqueID SS_AIR_TIMEOFDAY = 3203397129U;
        static const AkUniqueID SS_AIR_TURBULENCE = 4160247818U;
        static const AkUniqueID THROTTLE = 2995819693U;
        static const AkUniqueID UI = 1551306167U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBIANCES = 1404066300U;
        static const AkUniqueID BASS = 1291433344U;
        static const AkUniqueID CROWD_OS = 2077289713U;
        static const AkUniqueID DRUMS = 2726606664U;
        static const AkUniqueID DRUMS_B = 3042377063U;
        static const AkUniqueID ENGINE_P1 = 2631124783U;
        static const AkUniqueID ENGINE_P2 = 2631124780U;
        static const AkUniqueID ENGINE_P3 = 2631124781U;
        static const AkUniqueID ENGINE_P4 = 2631124778U;
        static const AkUniqueID ENGINES = 759487314U;
        static const AkUniqueID GUITAR = 3232836819U;
        static const AkUniqueID LEAD = 54442139U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID OTHER = 2376466361U;
        static const AkUniqueID P1 = 1635194252U;
        static const AkUniqueID P2 = 1635194255U;
        static const AkUniqueID P3 = 1635194254U;
        static const AkUniqueID P4 = 1635194249U;
        static const AkUniqueID PLAYERS = 2188949101U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__

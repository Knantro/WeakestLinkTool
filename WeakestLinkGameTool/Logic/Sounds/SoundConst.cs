namespace WeakestLinkGameTool.Logic.Sounds;

/// <summary>
/// Набор констант для звуков
/// </summary>
/// <remarks>
/// Содержит константы для:<br/>
/// • Громкостей звуков<br/>
/// • Плавных переходов<br/>
/// • Позиций A-B<br/>
/// • Ожиданий воспроизведения
/// </remarks>
public static class SoundConst {
    public const float GENERAL_BED_FADE_VOLUME                        = 0.1f;
    public const float GENERAL_BED_DEFAULT_VOLUME_COEFFICIENT         = 0.5f;
           
    public const long CLOSING_TITLES_LOOP_POSITION_A                  = 722_661;
    public const long CLOSING_TITLES_LOOP_POSITION_B                  = 1_050_138;
    public const long GENERAL_BED_LOOP_POSITION_A                     = 1_298_911;
    public const long GENERAL_BED_LOOP_POSITION_B                     = 3_925_882;
    public const long INTRO_LOOP_POSITION_A                           = 395_208;
    public const long INTRO_LOOP_POSITION_B                           = 1_028_058;
    public const long MAIN_STING_BED_LOOP_POSITION_A                  = 1_338_820;
    public const long MAIN_STING_BED_LOOP_POSITION_B                  = 3_963_760;
    public const long OPENING_TITLES_POSITION_MS                      = 15649;
    public const long PENALTY_SHOOTOUT_BED_LOOP_POSITION_A            = 1_415_819;
    public const long PENALTY_SHOOTOUT_BED_LOOP_POSITION_B            = 2_792_084;
    public const long PENALTY_SHOOTOUT_BED_V2_LOOP_POSITION_A         = 1_432_531;
    public const long PENALTY_SHOOTOUT_BED_V2_LOOP_POSITION_B         = 2_809_172;
    public const long SUDDEN_DEATH_BED_LOOP_POSITION_A                = 1_235_651;
    public const long SUDDEN_DEATH_BED_LOOP_POSITION_B                = 2_436_814;
    public const long SUDDEN_DEATH_BED_V2_LOOP_POSITION_A             = 1_260_379;
    public const long SUDDEN_DEATH_BED_V2_LOOP_POSITION_B             = 2_461_055;
    public const long VOTING_BED_LOOP_POSITION_A                      = 532_409;
    public const long VOTING_BED_LOOP_POSITION_B                      = 860_775;
    public const long WALK_OF_SHAME_LOOP_POSITION_A                   = 389_049;
    public const long WALK_OF_SHAME_LOOP_POSITION_B                   = 963_830;
    public const long WINNER_THEME_LOOP_POSITION_A                    = 615_364;
    public const long WINNER_THEME_LOOP_POSITION_B                    = 1_090_797;
    public const long WINNER_THEME_V2_LOOP_POSITION_A                 = 596_588;
    public const long WINNER_THEME_V2_LOOP_POSITION_B                 = 1_072_119;
       
    public const int CLOSING_TITLES_FADE                              = 100;
    public const int GENERAL_BED_FADE_IN_START_GAME                   = 3000;
    public const int INTRO_START_COOLDOWN                             = 3000;
    public const int OPENING_TITLES_FADE                              = 500;
    public const int OPENING_TITLES_AWAIT_TO_START_GAME               = 11000;
    public const int GENERAL_BED_FINAL_ROUND_FADE_VOLUME_DURATION     = 100;
    public const int GENERAL_BED_FINAL_ROUND_FADE_VOLUME_AWAIT_TIME   = 700;
    public const int GENERAL_BED_GENERAL_STING_FADE_VOLUME_DURATION   = 300;
    public const int GENERAL_BED_GENERAL_STING_FADE_VOLUME_AWAIT_TIME = 500;
    public const int GENERAL_BED_TARGET_STING_FADE_VOLUME_DURATION    = 100;
    public const int GENERAL_BED_TARGET_STING_FADE_VOLUME_AWAIT_TIME  = 1000;
    public const int GENERAL_BED_VOTING_STING_FADE_VOLUME_DURATION    = 100;
    public const int GENERAL_BED_VOTING_STING_FADE_VOLUME_AWAIT_TIME  = 500;
    public const int GENERAL_BED_FADE_OUT_TO_FINAL_ROUND_BED          = 200;
    public const int GENERAL_BED_AFTER_INTERVIEW_STING_DELAY          = 2000;
    public const int GENERAL_BED_WALK_OF_SHAME_DELAY                  = 1000;
    public const int TARGET_STING_AWAIT                               = 2000;
    public const int FINAL_BED_FADE_OUT_TO_WINNER_THEME               = 1500;
    public const int WINNER_THEME_FADE                                = 500;
    public const int VOTING_BED_START_COOLDOWN                        = 2300;
    public const int VOTING_BED_GENERAL_STING_FADE_OUT                = 2000;
    public const int WALK_OF_SHAME_AFTER_INTERVIEW_STING_FADE_OUT     = 3000;
}
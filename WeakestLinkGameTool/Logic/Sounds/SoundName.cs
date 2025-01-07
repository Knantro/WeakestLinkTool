namespace WeakestLinkGameTool.Logic.Sounds;

/// <summary>
/// 
/// </summary>
public static class SoundName {
    public const string AFTER_INTERVIEW_STING   = "after_interview_sting";
    public const string CLOSING_TITLES          = "closing_titles";
    public const string CLOSING_TITLES_STING    = "closing_titles_sting";
    public const string FINAL_BEGIN_STING       = "final_begin_sting";
    public const string GENERAL_BED             = "general_bed";
    public const string GENERAL_STING           = "general_sting";
    public const string INTRO                   = "intro";
    public const string MAIN_STING_BED          = "main_sting_bed";
    public const string OPENING_TITLES          = "opening_titles";
    public const string PENALTY_SHOOTOUT_BED    = "penalty_shootout_bed";
    public const string PENALTY_SHOOTOUT_BED_V2 = "penalty_shootout_bed_v2";
    public const string ROUND_90S               = "round_90s";
    public const string ROUND_100S              = "round_100s";
    public const string ROUND_110S              = "round_110s";
    public const string ROUND_120S              = "round_120s";
    public const string ROUND_130S              = "round_130s";
    public const string ROUND_140S              = "round_140s";
    public const string ROUND_150S              = "round_150s";
    public const string ROUND_160S              = "round_160s";
    public const string ROUND_170S              = "round_170s";
    public const string ROUND_180S              = "round_180s";
    public const string SHORT_STING             = "short_sting";
    public const string START_TIMER             = "start_timer";
    public const string STOP_TIMER              = "stop_timer";
    public const string SUDDEN_DEATH_BED        = "sudden_death_bed";
    public const string SUDDEN_DEATH_BED_V2     = "sudden_death_bed_v2";
    public const string TARGET_STING            = "target_sting";
    public const string VOTING_BED              = "voting_bed";
    public const string VOTING_STING            = "voting_sting";
    public const string WALK_OF_SHAME           = "walk_of_shame";
    public const string WINNER_THEME            = "winner_theme";
    public const string WINNER_THEME_V2         = "winner_theme_v2";
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static string FromRound(TimeSpan timeSpan) => $"round_{(int)timeSpan.TotalSeconds}s"; 
}
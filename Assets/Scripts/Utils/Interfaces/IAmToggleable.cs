namespace Scripts.Utils.Interfaces
{
    /// <summary>
    /// an interface for things that are meant to be toggled on or off
    /// </summary>
    public interface IAmToggleable
    {
        /// <summary>
        /// call this to toggle the thing on/off
        /// </summary>
        /// <param name="turnMeOn">should this be considered turned on or turned off?</param>
        void ToggleMe(bool turnMeOn);

        /// <summary>
        /// get whether or not this is toggled on
        /// </summary>
        /// <returns>true if this is toggled 'on', false otherwise.</returns>
        bool IsToggledOn();
    }
}
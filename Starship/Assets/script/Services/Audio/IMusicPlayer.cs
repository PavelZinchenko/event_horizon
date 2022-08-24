namespace Services.Audio
{
	public interface IMusicPlayer
	{
		float Volume { get; set; }
		void Mute(bool mute);
		void Pause();
		void Resume();
	}
}

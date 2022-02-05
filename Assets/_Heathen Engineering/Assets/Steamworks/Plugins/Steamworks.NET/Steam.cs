// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2019 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Steamworks {
	public static class SteamAPI {
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	Steam API setup & shutdown
		//
		//	These functions manage loading, initializing and shutdown of the steamclient.dll
		//
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//


		// SteamAPI_Init must be called before using any other API functions. If it fails, an
		// error message will be output to the debugger (or stderr) with further information.
		public static bool Init() {
			InteropHelp.TestIfPlatformSupported();

			bool ret = NativeMethods.SteamAPI_Init();

			// Steamworks.NET specific: We initialize the SteamAPI Context like this for now, but we need to do it
			// every time that Unity reloads binaries, so we also check if the pointers are available and initialized
			// before each call to any interface functions. That is in InteropHelp.cs
			if (ret) {
				ret = CSteamAPIContext.Init();
			}

			if (ret) {
				CallbackDispatcher.Initialize();
			}

			return ret;
		}

		// SteamAPI_Shutdown should be called during process shutdown if possible.
		public static void Shutdown() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_Shutdown();
			CSteamAPIContext.Clear();
			CallbackDispatcher.Shutdown();
		}

		// SteamAPI_RestartAppIfNecessary ensures that your executable was launched through Steam.
		//
		// Returns true if the current process should terminate. Steam is now re-launching your application.
		//
		// Returns false if no action needs to be taken. This means that your executable was started through
		// the Steam client, or a steam_appid.txt file is present in your game's directory (for development).
		// Your current process should continue if false is returned.
		//
		// NOTE: If you use the Steam DRM wrapper on your primary executable file, this check is unnecessary
		// since the DRM wrapper will ensure that your application was launched properly through Steam.
		public static bool RestartAppIfNecessary(AppId_t unOwnAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_RestartAppIfNecessary(unOwnAppID);
		}

		// Many Steam API functions allocate a small amount of thread-local memory for parameter storage.
		// SteamAPI_ReleaseCurrentThreadMemory() will free API memory associated with the calling thread.
		// This function is also called automatically by SteamAPI_RunCallbacks(), so a single-threaded
		// program never needs to explicitly call this function.
		public static void ReleaseCurrentThreadMemory() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_ReleaseCurrentThreadMemory();
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	steam callback and call-result helpers
		//
		//	The following macros and classes are used to register your application for
		//	callbacks and call-results, which are delivered in a predictable manner.
		//
		//	STEAM_CALLBACK macros are meant for use inside of a C++ class definition.
		//	They map a Steam notification callback directly to a class member function
		//	which is automatically prototyped as "void func( callback_type *pParam )".
		//
		//	CCallResult is used with specific Steam APIs that return "result handles".
		//	The handle can be passed to a CCallResult object's Set function, along with
		//	an object pointer and member-function pointer. The member function will
		//	be executed once the results of the Steam API call are available.
		//
		//	CCallback and CCallbackManual classes can be used instead of STEAM_CALLBACK
		//	macros if you require finer control over registration and unregistration.
		//
		//	Callbacks and call-results are queued automatically and are only
		//	delivered/executed when your application calls SteamAPI_RunCallbacks().
		//
		//	Note that there is an alternative, lower level callback dispatch mechanism.
		//	See SteamAPI_ManualDispatch_Init
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//

		// Dispatch all queued Steamworks callbacks.
		//
		// This is safe to call from multiple threads simultaneously,
		// but if you choose to do this, callback code could be executed on any thread.
		// One alternative is to call SteamAPI_RunCallbacks from the main thread only,
		// and call SteamAPI_ReleaseCurrentThreadMemory regularly on other threads.
		public static void RunCallbacks() {
			CallbackDispatcher.RunFrame(false);
		}

		//----------------------------------------------------------------------------------------------------------------------------------------------------------//
		//	steamclient.dll private wrapper functions
		//
		//	The following functions are part of abstracting API access to the steamclient.dll, but should only be used in very specific cases
		//----------------------------------------------------------------------------------------------------------------------------------------------------------//

		// SteamAPI_IsSteamRunning() returns true if Steam is currently running
		public static bool IsSteamRunning() {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_IsSteamRunning();
		}

		// returns the pipe we are communicating to Steam with
		public static HSteamPipe GetHSteamPipe() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamAPI_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamAPI_GetHSteamUser();
		}
	}

	public static class GameServer {
		// Initialize SteamGameServer client and interface objects, and set server properties which may not be changed.
		//
		// After calling this function, you should set any additional server parameters, and then
		// call ISteamGameServer::LogOnAnonymous() or ISteamGameServer::LogOn()
		//
		// - unIP will usually be zero.  If you are on a machine with multiple IP addresses, you can pass a non-zero
		//   value here and the relevant sockets will be bound to that IP.  This can be used to ensure that
		//   the IP you desire is the one used in the server browser.
		// - usGamePort is the port that clients will connect to for gameplay.  You will usually open up your
		//   own socket bound to this port.
		// - usQueryPort is the port that will manage server browser related duties and info
		//		pings from clients.  If you pass MASTERSERVERUPDATERPORT_USEGAMESOCKETSHARE for usQueryPort, then it
		//		will use "GameSocketShare" mode, which means that the game is responsible for sending and receiving
		//		UDP packets for the master  server updater. See references to GameSocketShare in isteamgameserver.h.
		// - The version string should be in the form x.x.x.x, and is used by the master server to detect when the
		//		server is out of date.  (Only servers with the latest version will be listed.)
		public static bool Init(uint unIP, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString) {
			InteropHelp.TestIfPlatformSupported();

			bool ret;
			using (var pchVersionString2 = new InteropHelp.UTF8StringHandle(pchVersionString)) {
				//ret = NativeMethods.SteamGameServer_Init(unIP, usGamePort, usQueryPort, eServerMode, pchVersionString2);
				ret = NativeMethods.SteamInternal_GameServer_Init(unIP, 0, usGamePort, usQueryPort, eServerMode, pchVersionString2);
			}

			// Steamworks.NET specific: We initialize the SteamAPI Context like this for now, but we need to do it
			// every time that Unity reloads binaries, so we also check if the pointers are available and initialized
			// before each call to any interface functions. That is in InteropHelp.cs
			if (ret) {
				ret = CSteamGameServerAPIContext.Init();
			}

			if (ret) {
				CallbackDispatcher.Initialize();
			}

			return ret;
		}

		// Shutdown SteamGameSeverXxx interfaces, log out, and free resources.
		public static void Shutdown() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_Shutdown();
			CSteamGameServerAPIContext.Clear();
			CallbackDispatcher.Shutdown();
		}

		public static void RunCallbacks() {
			CallbackDispatcher.RunFrame(true);
		}

		// Most Steam API functions allocate some amount of thread-local memory for
		// parameter storage. Calling SteamGameServer_ReleaseCurrentThreadMemory()
		// will free all API-related memory associated with the calling thread.
		// This memory is released automatically by SteamGameServer_RunCallbacks(),
		// so single-threaded servers do not need to explicitly call this function.
		public static void ReleaseCurrentThreadMemory() {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_ReleaseCurrentThreadMemory();
		}

		public static bool BSecure() {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamGameServer_BSecure();
		}

		public static CSteamID GetSteamID() {
			InteropHelp.TestIfPlatformSupported();
			return (CSteamID)NativeMethods.SteamGameServer_GetSteamID();
		}

		public static HSteamPipe GetHSteamPipe() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamGameServer_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser() {
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamGameServer_GetHSteamUser();
		}
	}

	public static class SteamEncryptedAppTicket {
		public static bool BDecryptTicket(byte[] rgubTicketEncrypted, uint cubTicketEncrypted, byte[] rgubTicketDecrypted, ref uint pcubTicketDecrypted, byte[] rgubKey, int cubKey) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BDecryptTicket(rgubTicketEncrypted, cubTicketEncrypted, rgubTicketDecrypted, ref pcubTicketDecrypted, rgubKey, cubKey);
		}

		public static bool BIsTicketForApp(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, AppId_t nAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BIsTicketForApp(rgubTicketDecrypted, cubTicketDecrypted, nAppID);
		}

		public static uint GetTicketIssueTime(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_GetTicketIssueTime(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static void GetTicketSteamID(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, out CSteamID psteamID) {
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamEncryptedAppTicket_GetTicketSteamID(rgubTicketDecrypted, cubTicketDecrypted, out psteamID);
		}

		public static uint GetTicketAppID(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_GetTicketAppID(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static bool BUserOwnsAppInTicket(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, AppId_t nAppID) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BUserOwnsAppInTicket(rgubTicketDecrypted, cubTicketDecrypted, nAppID);
		}

		public static bool BUserIsVacBanned(byte[] rgubTicketDecrypted, uint cubTicketDecrypted) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BUserIsVacBanned(rgubTicketDecrypted, cubTicketDecrypted);
		}

		public static byte[] GetUserVariableData(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, out uint pcubUserData) {
			InteropHelp.TestIfPlatformSupported();
			IntPtr punSecretData = NativeMethods.SteamEncryptedAppTicket_GetUserVariableData(rgubTicketDecrypted, cubTicketDecrypted, out pcubUserData);
			byte[] ret = new byte[pcubUserData];
			System.Runtime.InteropServices.Marshal.Copy(punSecretData, ret, 0, (int)pcubUserData);
			return ret;
		}

		public static bool BIsTicketSigned(byte[] rgubTicketDecrypted, uint cubTicketDecrypted, byte[] pubRSAKey, uint cubRSAKey) {
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamEncryptedAppTicket_BIsTicketSigned(rgubTicketDecrypted, cubTicketDecrypted, pubRSAKey, cubRSAKey);
		}
	}

	internal static class CSteamAPIContext {
		internal static void Clear() {
			m_pSteamClient = IntPtr.Zero;
			m_pSteamUser = IntPtr.Zero;
			m_pSteamFriends = IntPtr.Zero;
			m_pSteamUtils = IntPtr.Zero;
			m_pSteamMatchmaking = IntPtr.Zero;
			m_pSteamUserStats = IntPtr.Zero;
			m_pSteamApps = IntPtr.Zero;
			m_pSteamMatchmakingServers = IntPtr.Zero;
			m_pSteamNetworking = IntPtr.Zero;
			m_pSteamRemoteStorage = IntPtr.Zero;
			m_pSteamHTTP = IntPtr.Zero;
			m_pSteamScreenshots = IntPtr.Zero;
			m_pSteamGameSearch = IntPtr.Zero;
			m_pSteamMusic = IntPtr.Zero;
			m_pController = IntPtr.Zero;
			m_pSteamUGC = IntPtr.Zero;
			m_pSteamAppList = IntPtr.Zero;
			m_pSteamMusic = IntPtr.Zero;
			m_pSteamMusicRemote = IntPtr.Zero;
			m_pSteamHTMLSurface = IntPtr.Zero;
			m_pSteamInventory = IntPtr.Zero;
			m_pSteamVideo = IntPtr.Zero;
			m_pSteamParentalSettings = IntPtr.Zero;
			m_pSteamInput = IntPtr.Zero;
			m_pSteamParties = IntPtr.Zero;
			m_pSteamRemotePlay = IntPtr.Zero;
			m_pSteamNetworkingUtils = IntPtr.Zero;
			m_pSteamNetworkingSockets = IntPtr.Zero;
			m_pSteamNetworkingMessages = IntPtr.Zero;
		}

		internal static bool Init() {
			HSteamUser hSteamUser = SteamAPI.GetHSteamUser();
			HSteamPipe hSteamPipe = SteamAPI.GetHSteamPipe();
			if (hSteamPipe == (HSteamPipe)0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMCLIENT_INTERFACE_VERSION)) {
				m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(pchVersionString);
			}

			if (m_pSteamClient == IntPtr.Zero) { return false; }

			m_pSteamUser = SteamClient.GetISteamUser(hSteamUser, hSteamPipe, Constants.STEAMUSER_INTERFACE_VERSION);
			if (m_pSteamUser == IntPtr.Zero) { return false; }

			m_pSteamFriends = SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, Constants.STEAMFRIENDS_INTERFACE_VERSION);
			if (m_pSteamFriends == IntPtr.Zero) { return false; }

			m_pSteamUtils = SteamClient.GetISteamUtils(hSteamPipe, Constants.STEAMUTILS_INTERFACE_VERSION);
			if (m_pSteamUtils == IntPtr.Zero) { return false; }

			m_pSteamMatchmaking = SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, Constants.STEAMMATCHMAKING_INTERFACE_VERSION);
			if (m_pSteamMatchmaking == IntPtr.Zero) { return false; }

			m_pSteamMatchmakingServers = SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, Constants.STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION);
			if (m_pSteamMatchmakingServers == IntPtr.Zero) { return false; }

			m_pSteamUserStats = SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, Constants.STEAMUSERSTATS_INTERFACE_VERSION);
			if (m_pSteamUserStats == IntPtr.Zero) { return false; }

			m_pSteamApps = SteamClient.GetISteamApps(hSteamUser, hSteamPipe, Constants.STEAMAPPS_INTERFACE_VERSION);
			if (m_pSteamApps == IntPtr.Zero) { return false; }

			m_pSteamNetworking = SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, Constants.STEAMNETWORKING_INTERFACE_VERSION);
			if (m_pSteamNetworking == IntPtr.Zero) { return false; }

			m_pSteamRemoteStorage = SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, Constants.STEAMREMOTESTORAGE_INTERFACE_VERSION);
			if (m_pSteamRemoteStorage == IntPtr.Zero) { return false; }

			m_pSteamScreenshots = SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, Constants.STEAMSCREENSHOTS_INTERFACE_VERSION);
			if (m_pSteamScreenshots == IntPtr.Zero) { return false; }

			m_pSteamGameSearch = SteamClient.GetISteamGameSearch(hSteamUser, hSteamPipe, Constants.STEAMGAMESEARCH_INTERFACE_VERSION);
			if (m_pSteamGameSearch == IntPtr.Zero) { return false; }

			m_pSteamHTTP = SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, Constants.STEAMHTTP_INTERFACE_VERSION);
			if (m_pSteamHTTP == IntPtr.Zero) { return false; }

			m_pSteamUGC = SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, Constants.STEAMUGC_INTERFACE_VERSION);
			if (m_pSteamUGC == IntPtr.Zero) { return false; }

			m_pSteamAppList = SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, Constants.STEAMAPPLIST_INTERFACE_VERSION);
			if (m_pSteamAppList == IntPtr.Zero) { return false; }

			m_pSteamMusic = SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, Constants.STEAMMUSIC_INTERFACE_VERSION);
			if (m_pSteamMusic == IntPtr.Zero) { return false; }

			m_pSteamMusicRemote = SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, Constants.STEAMMUSICREMOTE_INTERFACE_VERSION);
			if (m_pSteamMusicRemote == IntPtr.Zero) { return false; }

			m_pSteamHTMLSurface = SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, Constants.STEAMHTMLSURFACE_INTERFACE_VERSION);
			if (m_pSteamHTMLSurface == IntPtr.Zero) { return false; }

			m_pSteamInventory = SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, Constants.STEAMINVENTORY_INTERFACE_VERSION);
			if (m_pSteamInventory == IntPtr.Zero) { return false; }

			m_pSteamVideo = SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, Constants.STEAMVIDEO_INTERFACE_VERSION);
			if (m_pSteamVideo == IntPtr.Zero) { return false; }

			m_pSteamParentalSettings = SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, Constants.STEAMPARENTALSETTINGS_INTERFACE_VERSION);
			if (m_pSteamParentalSettings == IntPtr.Zero) { return false; }

			m_pSteamInput = SteamClient.GetISteamInput(hSteamUser, hSteamPipe, Constants.STEAMINPUT_INTERFACE_VERSION);
			if (m_pSteamInput == IntPtr.Zero) { return false; }

			m_pSteamParties = SteamClient.GetISteamParties(hSteamUser, hSteamPipe, Constants.STEAMPARTIES_INTERFACE_VERSION);
			if (m_pSteamParties == IntPtr.Zero) { return false; }

			m_pSteamRemotePlay = SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, Constants.STEAMREMOTEPLAY_INTERFACE_VERSION);
			if (m_pSteamRemotePlay == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingUtils =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) != IntPtr.Zero ?
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) :
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingUtils == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingSockets =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingSockets == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION))
			{
				m_pSteamNetworkingMessages =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingMessages == IntPtr.Zero) { return false; }

			return true;
		}

		internal static IntPtr GetSteamClient() { return m_pSteamClient; }
		internal static IntPtr GetSteamUser() { return m_pSteamUser; }
		internal static IntPtr GetSteamFriends() { return m_pSteamFriends; }
		internal static IntPtr GetSteamUtils() { return m_pSteamUtils; }
		internal static IntPtr GetSteamMatchmaking() { return m_pSteamMatchmaking; }
		internal static IntPtr GetSteamUserStats() { return m_pSteamUserStats; }
		internal static IntPtr GetSteamApps() { return m_pSteamApps; }
		internal static IntPtr GetSteamMatchmakingServers() { return m_pSteamMatchmakingServers; }
		internal static IntPtr GetSteamNetworking() { return m_pSteamNetworking; }
		internal static IntPtr GetSteamRemoteStorage() { return m_pSteamRemoteStorage; }
		internal static IntPtr GetSteamScreenshots() { return m_pSteamScreenshots; }
		internal static IntPtr GetSteamGameSearch() { return m_pSteamGameSearch; }
		internal static IntPtr GetSteamHTTP() { return m_pSteamHTTP; }
		internal static IntPtr GetSteamController() { return m_pController; }
		internal static IntPtr GetSteamUGC() { return m_pSteamUGC; }
		internal static IntPtr GetSteamAppList() { return m_pSteamAppList; }
		internal static IntPtr GetSteamMusic() { return m_pSteamMusic; }
		internal static IntPtr GetSteamMusicRemote() { return m_pSteamMusicRemote; }
		internal static IntPtr GetSteamHTMLSurface() { return m_pSteamHTMLSurface; }
		internal static IntPtr GetSteamInventory() { return m_pSteamInventory; }
		internal static IntPtr GetSteamVideo() { return m_pSteamVideo; }
		internal static IntPtr GetSteamParentalSettings() { return m_pSteamParentalSettings; }
		internal static IntPtr GetSteamInput() { return m_pSteamInput; }
		internal static IntPtr GetSteamParties() { return m_pSteamParties; }
		internal static IntPtr GetSteamRemotePlay() { return m_pSteamRemotePlay; }
		internal static IntPtr GetSteamNetworkingUtils() { return m_pSteamNetworkingUtils; }
		internal static IntPtr GetSteamNetworkingSockets() { return m_pSteamNetworkingSockets; }
		internal static IntPtr GetSteamNetworkingMessages() { return m_pSteamNetworkingMessages; }

		private static IntPtr m_pSteamClient;
		private static IntPtr m_pSteamUser;
		private static IntPtr m_pSteamFriends;
		private static IntPtr m_pSteamUtils;
		private static IntPtr m_pSteamMatchmaking;
		private static IntPtr m_pSteamUserStats;
		private static IntPtr m_pSteamApps;
		private static IntPtr m_pSteamMatchmakingServers;
		private static IntPtr m_pSteamNetworking;
		private static IntPtr m_pSteamRemoteStorage;
		private static IntPtr m_pSteamScreenshots;
		private static IntPtr m_pSteamGameSearch;
		private static IntPtr m_pSteamHTTP;
		private static IntPtr m_pController;
		private static IntPtr m_pSteamUGC;
		private static IntPtr m_pSteamAppList;
		private static IntPtr m_pSteamMusic;
		private static IntPtr m_pSteamMusicRemote;
		private static IntPtr m_pSteamHTMLSurface;
		private static IntPtr m_pSteamInventory;
		private static IntPtr m_pSteamVideo;
		private static IntPtr m_pSteamParentalSettings;
		private static IntPtr m_pSteamInput;
		private static IntPtr m_pSteamParties;
		private static IntPtr m_pSteamRemotePlay;
		private static IntPtr m_pSteamNetworkingUtils;
		private static IntPtr m_pSteamNetworkingSockets;
		private static IntPtr m_pSteamNetworkingMessages;
	}

	internal static class CSteamGameServerAPIContext {
		internal static void Clear() {
			m_pSteamClient = IntPtr.Zero;
			m_pSteamGameServer = IntPtr.Zero;
			m_pSteamUtils = IntPtr.Zero;
			m_pSteamNetworking = IntPtr.Zero;
			m_pSteamGameServerStats = IntPtr.Zero;
			m_pSteamHTTP = IntPtr.Zero;
			m_pSteamInventory = IntPtr.Zero;
			m_pSteamUGC = IntPtr.Zero;
			m_pSteamApps = IntPtr.Zero;
			m_pSteamNetworkingUtils = IntPtr.Zero;
			m_pSteamNetworkingSockets = IntPtr.Zero;
			m_pSteamNetworkingMessages = IntPtr.Zero;
	}

		internal static bool Init() {
			HSteamUser hSteamUser = GameServer.GetHSteamUser();
			HSteamPipe hSteamPipe = GameServer.GetHSteamPipe();
			if (hSteamPipe == (HSteamPipe)0) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMCLIENT_INTERFACE_VERSION)) {
				m_pSteamClient = NativeMethods.SteamInternal_CreateInterface(pchVersionString);
			}
			if (m_pSteamClient == IntPtr.Zero) { return false; }

			m_pSteamGameServer = SteamGameServerClient.GetISteamGameServer(hSteamUser, hSteamPipe, Constants.STEAMGAMESERVER_INTERFACE_VERSION);
			if (m_pSteamGameServer == IntPtr.Zero) { return false; }

			m_pSteamUtils = SteamGameServerClient.GetISteamUtils(hSteamPipe, Constants.STEAMUTILS_INTERFACE_VERSION);
			if (m_pSteamUtils == IntPtr.Zero) { return false; }

			m_pSteamNetworking = SteamGameServerClient.GetISteamNetworking(hSteamUser, hSteamPipe, Constants.STEAMNETWORKING_INTERFACE_VERSION);
			if (m_pSteamNetworking == IntPtr.Zero) { return false; }

			m_pSteamGameServerStats = SteamGameServerClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, Constants.STEAMGAMESERVERSTATS_INTERFACE_VERSION);
			if (m_pSteamGameServerStats == IntPtr.Zero) { return false; }

			m_pSteamHTTP = SteamGameServerClient.GetISteamHTTP(hSteamUser, hSteamPipe, Constants.STEAMHTTP_INTERFACE_VERSION);
			if (m_pSteamHTTP == IntPtr.Zero) { return false; }

			m_pSteamInventory = SteamGameServerClient.GetISteamInventory(hSteamUser, hSteamPipe, Constants.STEAMINVENTORY_INTERFACE_VERSION);
			if (m_pSteamInventory == IntPtr.Zero) { return false; }

			m_pSteamUGC = SteamGameServerClient.GetISteamUGC(hSteamUser, hSteamPipe, Constants.STEAMUGC_INTERFACE_VERSION);
			if (m_pSteamUGC == IntPtr.Zero) { return false; }

			m_pSteamApps = SteamGameServerClient.GetISteamApps(hSteamUser, hSteamPipe, Constants.STEAMAPPS_INTERFACE_VERSION);
			if (m_pSteamApps == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGUTILS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingUtils =
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) != IntPtr.Zero ?
					NativeMethods.SteamInternal_FindOrCreateUserInterface(hSteamUser, pchVersionString) :
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingUtils == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGSOCKETS_INTERFACE_VERSION))
			{
				m_pSteamNetworkingSockets =
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingSockets == IntPtr.Zero) { return false; }

			using (var pchVersionString = new InteropHelp.UTF8StringHandle(Constants.STEAMNETWORKINGMESSAGES_INTERFACE_VERSION))
			{
				m_pSteamNetworkingMessages =
					NativeMethods.SteamInternal_FindOrCreateGameServerInterface(hSteamUser, pchVersionString);
			}
			if (m_pSteamNetworkingMessages == IntPtr.Zero) { return false; }

			return true;
		}

		internal static IntPtr GetSteamClient() { return m_pSteamClient; }
		internal static IntPtr GetSteamGameServer() { return m_pSteamGameServer; }
		internal static IntPtr GetSteamUtils() { return m_pSteamUtils; }
		internal static IntPtr GetSteamNetworking() { return m_pSteamNetworking; }
		internal static IntPtr GetSteamGameServerStats() { return m_pSteamGameServerStats; }
		internal static IntPtr GetSteamHTTP() { return m_pSteamHTTP; }
		internal static IntPtr GetSteamInventory() { return m_pSteamInventory; }
		internal static IntPtr GetSteamUGC() { return m_pSteamUGC; }
		internal static IntPtr GetSteamApps() { return m_pSteamApps; }
		internal static IntPtr GetSteamNetworkingUtils() { return m_pSteamNetworkingUtils; }
		internal static IntPtr GetSteamNetworkingSockets() { return m_pSteamNetworkingSockets; }
		internal static IntPtr GetSteamNetworkingMessages() { return m_pSteamNetworkingMessages; }

		private static IntPtr m_pSteamClient;
		private static IntPtr m_pSteamGameServer;
		private static IntPtr m_pSteamUtils;
		private static IntPtr m_pSteamNetworking;
		private static IntPtr m_pSteamGameServerStats;
		private static IntPtr m_pSteamHTTP;
		private static IntPtr m_pSteamInventory;
		private static IntPtr m_pSteamUGC;
		private static IntPtr m_pSteamApps;
		private static IntPtr m_pSteamNetworkingUtils;
		private static IntPtr m_pSteamNetworkingSockets;
		private static IntPtr m_pSteamNetworkingMessages;
	}
}

#endif // !DISABLESTEAMWORKS

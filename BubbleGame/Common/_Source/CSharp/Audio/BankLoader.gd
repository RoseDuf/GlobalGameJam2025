extends Node


var banks := Array()

func _init():
	banks.append(FmodServer.load_bank("bank:/Master.strings.bank", FmodServer.FMOD_STUDIO_LOAD_BANK_NORMAL))
	banks.append(FmodServer.load_bank("bank:/Master.bank", FmodServer.FMOD_STUDIO_LOAD_BANK_NORMAL))
	banks.append(FmodServer.load_bank("bank:/Music.bank", FmodServer.FMOD_STUDIO_LOAD_BANK_NORMAL))

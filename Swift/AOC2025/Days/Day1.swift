import Playgrounds
import Foundation
import AOCLib

struct Safe {
	struct Turn {
		var toRight: Bool
		var count: Int
	}
	var turns: [Turn]
	
	var numberOfZeroes: Int {
		var pos = 50
		var count = 0
		turns.forEach {
			pos += ($0.toRight ? 1 : -1) * $0.count
			pos %= 100
			if pos == 0 {
				count += 1
			}
		}
		return count
	}
	
	var numberOfZeroes2: Int {
		var pos = 50
		var count = 0
		turns.forEach {
			let startPos = pos
			pos += ($0.toRight ? 1 : -1) * $0.count
			pos %= 100
			
			if pos == 0 {
				count += 1
			}
		}
		return count
	}
	
	static func load(_ filename: String) -> Safe {
		let lines = FileHelper.load(filename)!.compactMap { $0 }
		let turns: [Turn] = lines.compactMap {
			guard let dir = $0.first else { return nil }
			
			let numberString = $0.dropFirst()
			guard let num = Int(numberString) else { return nil }

			return .init(toRight: dir == "R", count: num)
		}
		return .init(turns: turns)
	}
}

#Playground {
	let safe = Safe.load("Day1_Test")
	print(safe.numberOfZeroes)
	
	let safe2 = Safe.load("Day1_Input")
	print(safe2.numberOfZeroes)
	
	let safe3 = Safe.load("Day1_Test")
	print(safe3.numberOfZeroes2)
	
		//	let safe2 = Safe.load("Day1_Input")
//	print(safe2.numberOfZeroes2)
}

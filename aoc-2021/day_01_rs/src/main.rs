use reqwest::header::COOKIE;
//use std::str::Lines;

fn main() -> Result<(), reqwest::Error> {

	let client = reqwest::blocking::Client::new();

	let res = client
		.get("https://adventofcode.com/2021/day/1/input")
		.header(COOKIE, "session=53616c7465645f5f15c3b399a1b4cb0c87c1d92afa8cf82daccd8ec71cdd9c80baeeda5ef47e2aeaf23a8ba42f97c022")
		.send()?;

	//println!("stuff: {:?}", res.text()?.lines());

	let mut last = i32::MAX;
	let mut inc  = 0;

	for line in res.text()?.lines() {
		let cur = line.parse::<i32>().unwrap();

		if cur > last {
			inc += 1;
		}

		last = cur;
	}

	println!("{}", inc);
	
	Ok(())
}

"use strict"

import { figlet } from "./figlet.js";

(function main() {
	//gets the element that displays the number of titles
	let num_element = document.getElementById("ascii-number");
	if (!num_element) console.error("Element with ID 'ascii-number' not found.");

    //manually add title to the db, by typing it in the input-text box
	document.getElementById("save-btn").addEventListener("click", async () => {
		let textInput = document.getElementById("input-text");
		
		let is_insert_ok = await send_title(num_element, textInput.value)

		if (is_insert_ok) textInput.value = "";
	});

    //quickly add a title to the db
	document.getElementById("quick-add-btn").addEventListener("click", async () => {
		try {
			const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
			if (!tab.url.toLowerCase().startsWith("https://aniwave.to")) return;
			
			const results = await chrome.scripting.executeScript({
				target: { tabId: tab.id },
				injectImmediately: true,
				func: get_inner_text,
				args: ["div.names"],
			});

			let is_insert_ok = await send_title(num_element, results[0].result.split(";")[0]);
		} catch (error) {
			console.log("There was an error injecting script : \n" + error.message);
		}
	});

	fetch_ascii_num(num_element);
})();

//calls backend service to insert title into the db
async function send_title(num_element, title) {
	try {
		const response = await fetch(
			`http://localhost:56665/titlelist/add?title=${encodeURIComponent(title)}`,
			{ method: "POST" }
		);
	
		if (!response.ok) throw new Error(`Failed to fetch. Status: ${response.status}`);
	
		const data = await response.json();
	
		let returnedID = data.id;
		if (returnedID) localStorage.setItem("latest_ID", returnedID);
	
		fetch_ascii_num(num_element);
		return true
	} catch (error) {
		return false
	}
}

//convert text to ASCII
function fetch_ascii_num(num_element) {
	return figlet(
		`${localStorage.getItem("latest_ID")}`,
		"Small Slant",
		function (err, text) {
			if (err) {
				console.log("something went wrong...");
				console.dir(err);
				return;
			}
			num_element.innerText = text;
		}
	);
}

//gets innerHTML of an element
function get_inner_text(selector) {
	selector = document.querySelector(selector);
	if (!selector) return "ERROR: querySelector failed to find node";
	return selector.innerHTML;
};

# FastMessageRouter

This library makes use of generic static classes behind the scenes to route messages by type while avoiding the dictionary lookups found in other common implementations. It is at an early stage of development and has not yet been tested thoroughly, but so far is showing potential speed-ups on the order of 2-10x depending on the platform.
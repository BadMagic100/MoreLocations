# MoreLocations

A Hollow Knight randomizer connection that adds various additional locations to the pool, which may not necessarily have corresponding items.
This is intended to give another way to thin out shops to account for added things like duplicates without needing to take other measures, e.g.
full flexible count or condensing down mask shards to double mask shards, or further thinning out shops in addition to those type of measures.

Note that this mod may be incompatible with some settings in LoreMaster/Lore Randomizer.

## Available Settings

This mod offers several additional settings, which are each described in more detail below. The mod comes with recommended default settings waiting
for you to have a balanced experience when first using the connection.

### Miscellaneous Locations

The Miscellaneous Locations sub-menu contains several individually toggleable one-off locations, which do not add any items to the randomization pool.

* **Swim**: I always thought it was a bit odd that Elevator Pass got a vanilla location, but Swim never did. I've fixed that now;
  there is someone in Hallownest renowned for their swimming expertise who can teach you.
* **Stag Nest Egg**: I hope you can figure out where this is by the name.
* **Baldur Shell Chest**: Did you know that all the geo chests in the game have holes in the bottom? This is the only one you can usually
  go through though. Hope you brought benchwarp for when the shiny goes down there!

### Lemm Shop

Randomizes Lemm's shop. As usual, you will trade him relics, but what you get back in return might not be geo. Settings are included to control
the total number required to pay costs of each relic type, as well as the number of relic geo items which will be shuffled in the pool.

Logic will ensure that you will always have enough relics available to pay all relic costs, so you may safely purchase items in any order. This also
includes the additional tolerance as defined by your settings, similar to other costs. Note that doing so before you can purchase all items may be
considered a sequence break by logic.

### Junk Shop

Adds a shop at Fluke Hermit above Junk Pit which can have one or more costs of any type. Settings are included to select how many costs each item can have.
Settings are selected based on what costs are available to randomization:

* Grub, Essence, and Geo costs are always included.
* If Egg Shop is enabled, egg costs can be included. Egg costs are cumulatively shared between Jiji and Fluke Hermit.
* If Lemm Shop is enabled, relic costs can be included.
* If TheRealJournalRando is enabled, enemy kill costs can be included.
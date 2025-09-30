Parking Lot [   ]

happy path: 
person A wants to trade item 1 for person B's item 2

1. person A adds the item to the website
2. person A creates a trade
[trade status: pending]
3. person A add the item to the trade
4. person A makes the trade public
[trade status: visible]

4.5. person B looks on the website and finds the trade

5. person B adds the item (Samsung) to the website
6. person B makes an offer on person A's trade
7. person B adds the item (Samsung) to the offer

8. person A accepts the offer
[trade status: accepted]
9. backend exchanges contact information of both parties
10. they meet up and trade (in real life)
11. person A marks trade as complete
12. person B marks trade as complete
[trade status: complete]

13. move item 2 to person A's inventory
14. move item 1 to person B's inventory
15. move trade to trade archive

---

conflicting trade:

1. person A adds the item to the website
2. person A creates a trade
[trade status: pending]
3. person A add the item to the trade
4. person A makes the trade public
[trade status: visible]

4.5. person B looks on the website and finds the trade

5. person B adds the item (Samsung) to the website
6. person B makes an offer on person A's trade
7. person B adds the item (Samsung) to the offer

8. person A accepts the offer
[trade status: accepted]
9. backend exchanges contact information of both parties
10. they meet up and trade (in real life)
11. person A marks trade as complete
12. person B marks trade as rejected

13. hide trade?
[trade status: pending]
14. remove offer?

---

person A adds the item to the website
person B adds the item to the website
person B browses items up for trade
person B makes an trade offer(trade object) on person A's item
trade object state is set to "proposal"
person A accepts the offer
trade object is set to "trading"
user info becomes visible to both parties based on "trading" state
they meet up and trade (in real life) "gps" initiates sceduled trade event
person A confirms at location and time
person B confirms at location and time
inventory items are exchanged
trade is marked as complete






-------------- alternative user stories:
User Stories
1. Listing Items

As a user, I want to list my iPhone 10 in the TradeHub system with details (condition, description, and availability) so that other users can see it is available for trade.

As a user, I want to list my Samsung Galaxy 10 in the TradeHub system with its details so I can offer it in an exchange.

2. Browsing & Discovering

As a user, I want to browse items available for trade in the marketplace so that I can find potential matches for my iPhone 10.

As a user, I want to search specifically for phones of similar value (like a Samsung Galaxy 10) so that I can propose a fair trade.

3. Proposing a Trade

As a user (iPhone owner), I want to propose a trade offering my iPhone 10 in exchange for the Samsung Galaxy 10 so that the other user can review the deal.

As a user (Galaxy owner), I want to receive trade proposals and review the details (item condition, notes, owner reputation) before deciding.

4. Accepting or Rejecting

As a user (Galaxy owner), I want to accept the trade proposal for the iPhone 10 so that the system marks the trade as "accepted" and guides us toward completion.

As a user (Galaxy owner), I want to reject the trade proposal if I don’t find it fair or if I change my mind.

5. Completing the Trade

As both users, we want the system to notify us when a trade is accepted and provide a secure way to arrange delivery or meeting so that the transaction is finalized.

As both users, we want the system to mark the trade as "completed" once we confirm the exchange has happened successfully.

6. Extra: Fairness & Trust

As a user, I want to view ratings or feedback about the other trader so that I feel confident the trade will go smoothly.

As a user, I want to leave feedback after the trade to help future users judge reliability.
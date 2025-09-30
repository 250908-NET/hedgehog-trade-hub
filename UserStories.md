# User Stories

Parking Lot [   ]

## 0. Login

1. As a user, I want to create an account so I can start trading with other users.

2. As a user, I want to log into my account so I can start trading with other users.

3. As a user, I want to modify my account details (about me, email, username, password) so that I can update my account with accurate information.

4. As a user, I want to delete my account so I can stop trading with other users.

## 1. Listing Items

1. As a user, I want to list my item so that I can trade it with other users.

2. As a user, I want to add details to my item (condition, description, and availability) so that other people can make an informed decision on trading.

3. As a user, I want to create a publicly visible trade so that other users can trade with me.

4. As a user, I want to add existing items to a trade so that I can trade them with other users.

## 2. Browsing & Discovering

1. As a user, I want to browse items available for trade in the marketplace so that I can find items to trade.

2. As a user, I want to search specifically for items of similar value so that I can propose a fair trade.

3. As a user, I want to search using keywords so that I can find something I want to trade for.

## 3. Proposing a Trade

1. As a user, I want to create an offer for an existing trade so that the other user can review the deal.

2. As a user, I want to add existing items to my offer so that the other user can make a decision on the deal.

3. As a user, I want to receive trade proposals and review the details (item condition, notes, owner reputation) before deciding so that I have accurate information about the trade.

## 4. Accepting or Rejecting

1. As a user (Galaxy owner), I want to accept the trade proposal for the iPhone 10 so that the system marks the trade as "accepted" and guides us toward completion.

2. As a user (Galaxy owner), I want to reject the trade proposal if I don’t find it fair or if I change my mind.

## 5. Completing the Trade

1. As both users, we want the system to mark the trade as "completed" once we confirm the exchange has happened successfully so that the trade can be completed.

## 6. Administraton

1. As an admin, I want to create, modify, and delete user accounts so that I can do administrative tasks.

2. As an admin, I want to place and lift restrictions on users so that bad user behavior can be prevented.

3. As an admin, I want to modify and delete items so that disallowed items can be removed.

4. As an admin, I want to modify and delete trades so that trades of disallowed items can be stopped.

## Ice Bucket

1. As a user, I want to list a meeting place for the trade so that I can easily organize a trade.

2. As a user, I want to get notifications so that I can know when the status of a trade changes.

3. As a user, I want to get a notification when a new item is added that meets my search criteria so that I can find new trades I am interested in.

4. As a user, I want to view ratings or feedback about the other trader so that I feel confident the trade will go smoothly.

5. As a user, I want to leave feedback after the trade to help future users judge reliability.

6. As a user, I want to ensure unauthorized people cannot modify my trades and items so that my information is secure. (JWT?)

## Sequences

### sequence 1: happy path

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

### sequence 2: rejected trade

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
12. person B marks trade as rejected

13. hide trade?
[trade status: pending]
14. remove offer?

---

1. person A adds the item to the website
2. person B adds the item (Samsung) to the website
3. person B makes an trade offer(proposal state) on person A's item, which makes a trade object
4. person A accepts the offer(Trading state)
5. backend exchanges contact information of both parties
6. they meet up and trade (in real life) "gps" initiates sceduled trade event
7. person A confirms at location
8. person B confirms at location
9. inventory items are exchanged
9. trade is marked as complete


------------


Register (Sign Up)

As a new user, I want to create an account with a username, email, and password so that I can access the TradeHub platform.

As a new user, I want my password to be stored securely (hashed, not plain text) so that my personal information is protected.

As a new user, I want to receive feedback if my email is already taken or my password is too weak so that I can correct it and successfully register.

Login (Sign In)

As a registered user, I want to log in using my email and password so that I can access my account.

As a registered user, I want the system to verify my credentials securely so that only I can access my account.

As a registered user, I want to receive a secure session token (JWT) so that I can remain logged in and perform actions without re-entering credentials every time.

As a registered user, I want to see a clear error message if I provide invalid credentials so that I understand what went wrong.





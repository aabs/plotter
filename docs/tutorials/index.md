# Tutorials

## 1. Scene management

```bash
novel init
novel participant add Mara
novel location add "Boarding house"
novel scene add S001
novel scene set S001 --date-time 1928-06-14T08:10 --participant Mara --location "Boarding house" --title "Mara wakes" --status draft --notes "The telegram has arrived."
novel scene show S001
```

## 2. Timeline and manuscript order

```bash
novel timeline --from 1928-06-14 --to 1928-06-15
novel scenes timeline
novel scene list --order manuscript
```

## 3. Character continuity

```bash
novel character timeline Mara
novel scenes lanes --characters Mara,Elias,Vale --date 1928-06-14
novel lanes --group "The conspiracy"
```

## 4. Location occupancy

```bash
novel location show "Hotel ballroom"
novel locations list --occupancy
novel where --at 1928-06-14T15:00
```

## 5. Plot threads

```bash
novel plot add "Missing ledger" --description "The stolen ledger"
novel threads matrix
novel thread show "Missing ledger"
```

## 6. Audits and travel

```bash
novel continuity gaps
novel audit all
novel travel Mara
```

## 7. Exports

```bash
novel export --format json
novel export --format csv
novel export --format markdown
novel export graph --by participants
novel export graph --by locations --format mermaid
```

## 8. Calendar

```bash
novel calendar --day --date 1928-06-14
novel calendar --month --date 1928-06
```

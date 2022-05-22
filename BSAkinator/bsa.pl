:- encoding(utf8).

:- dynamic rarity/2.
:- dynamic race/2.
:- dynamic speed/2.
:- dynamic gadget_attack/2.
:- dynamic gender/2.
:- dynamic attack/2.
:- dynamic short_name/2.
:- dynamic super_damage/2.

read_str(A, N) :- read_str(A, N, 0).
read_str(A,N,Flag):-get0(X), not(X = -1), r_str(X,A,[],N,0,Flag).
r_str(-1,A,A,N,N,1):-!.
r_str(10,A,A,N,N,0):-!.
r_str(X,A,B,N,K,Flag):-K1 is K+1,append(B,[X],B1),get0(X1),r_str(X1,A,B1,N,K1,Flag).

write_str([]):-!.
write_str([H|Tail]):-put(H),write_str(Tail).

%%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%% %%%%

read_brawlers :- 
    read_str(CharStr, Flag), not(Flag = 1), name(Char, CharStr), 
    read(Rarity), read(Race), read(Speed), read(GadgetAttack), read(Gender), read(Attack), read(ShortName), read(SuperDamage), get0(_),
    assertz(rarity(Char, Rarity)), assertz(race(Char, Race)), assertz(speed(Char, Speed)), assertz(gadget_attack(Char, GadgetAttack)),
    assertz(gender(Char, Gender)), assertz(attack(Char, Attack)), assertz(short_name(Char, ShortName)), assertz(super_damage(Char, SuperDamage)),
    read_brawlers, !.
read_brawlers:- !.

questionRarity(X) :-
    write("Насколько редкий Ваш персонаж?|7"), nl, 
    write("Начальный/Путь к славе"), nl,
    write("Редкий"), nl,
    write("СверхРедкий"), nl,
    write("Эпический"), nl,
    write("Мифический"), nl,
    write("Легендарный"), nl,
    write("Хроматический"), nl,
    read(X).

questionRace(X) :-
    write("Какова раса Вашего персонажа?|4"), nl,
    write("Человек"), nl,
    write("Робот"), nl,
    write("Животное"), nl,
    write("Другое"), nl,
    read(X).

questionSpeed(X) :-
    write("Какая скорость у Вашего персонажа?|5"), nl,
    write("Очень низкая"), nl,
    write("Низкая"), nl,
    write("Нормальная"), nl,
    write("Высокая"), nl,
    write("Очень высокая"), nl,
    read(X).

questionGadgetAttack(X) :-
    write("Гаджет влияет на основную атаку?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

questionGender(X) :-
    write("Ваш персонаж мужского пола?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

questionAttack(X) :-
    write("Ваш персонаж атакует сплешем?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

questionAttack(X) :-
    write("Ваш персонаж атакует сплэшем?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

questionShortName(X) :-
    write("Имя Вашего персонажа короче 4 букв?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

questionSuperDamage(X) :-
    write("Ульта Вашего персонажа может нанести урон?|2"), nl,
    write("Да"), nl,
    write("Нет"), nl,
    read(X).

% Проверка на существование
exists(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) :-
    rarity(X, Rarity), race(X, Race), speed(X, Speed), gadget_attack(X, GadgetAttack),
    gender(X, Gender), attack(X, Attack), short_name(X, ShortName), super_damage(X, SuperDamage), !.

% Проверка, что существует единственный ответ
getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) :-
    (nonvar(Rarity) -> rarity(X, Rarity) ; true), 
    (nonvar(Race) -> race(X, Race) ; true),
    (nonvar(Speed) -> speed(X, Speed) ; true), 
    (nonvar(GadgetAttack) -> gadget_attack(X, GadgetAttack) ; true), 
    (nonvar(Gender) -> gender(X, Gender) ; true),
    (nonvar(Attack) -> attack(X, Attack) ; true),
    (nonvar(ShortName) -> short_name(X, ShortName) ; true),
    (nonvar(SuperDamage) -> super_damage(X, SuperDamage) ; true),
    ((
        (nonvar(Rarity) -> rarity(AltX, Rarity) ; true), 
        (nonvar(Race) -> race(AltX, Race) ; true),
        (nonvar(Speed) -> speed(AltX, Speed) ; true), 
        (nonvar(GadgetAttack) -> gadget_attack(AltX, GadgetAttack) ; true), 
        (nonvar(Gender) -> gender(AltX, Gender) ; true),
        (nonvar(Attack) -> attack(AltX, Attack) ; true),
        (nonvar(ShortName) -> short_name(AltX, ShortName) ; true),
        (nonvar(SuperDamage) -> super_damage(AltX, SuperDamage) ; true),
    
        not(AltX = X)
    ) -> fail ; !).

find :-
    questionRarity(Rarity), (getSingleAnswer(X, Rarity, _, _, _, _, _, _, _) -> found(X, Rarity, _, _, _, _, _, _, _), ! ; 
    (questionRace(Race), (getSingleAnswer(X, Rarity, Race, _, _, _, _, _, _) -> found(X, Rarity, Race, _, _, _, _, _, _), ! ;
    (questionSpeed(Speed), (getSingleAnswer(X, Rarity, Race, Speed, _, _, _, _, _) -> found(X, Rarity, Race, Speed, _, _, _, _, _), ! ; 
    (questionGadgetAttack(GadgetAttack), (getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, _, _, _, _) -> found(X, Rarity, Race, Speed, GadgetAttack, _, _, _, _), ! ; 
    (questionGender(Gender), (getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, Gender, _, _, _) -> found(X, Rarity, Race, Speed, GadgetAttack, Gender, _, _, _), ! ;
    (questionAttack(Attack), (getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, _, _) -> found(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, _, _), ! ; 
    (questionShortName(ShortName), (getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, _) -> found(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, _), ! ;
    (questionSuperDamage(SuperDamage), (getSingleAnswer(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) -> found(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage), ! ; 
    not_found(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage)
    ))))))))))))))).

% Нашли единственный ответ
found(X, Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) :-
    write("+Ваш персонаж — "), write(X), write("?"), nl, read(Ans),
    Ans =\= 1 -> addChar(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage); true.

% Не нашли ответ
not_found(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) :-
    write("-Я в замешательстве... Добавить нового персонажа?"), nl, read(Ans),
    Ans = 1 -> addChar(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage); true.

addChar(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) :-
    (var(Rarity) -> questionRarity(Rarity) ; true), 
    (var(Race) -> questionRace(Race) ; true),
    (var(Speed) -> questionSpeed(Speed) ; true), 
    (var(GadgetAttack) -> questionGadgetAttack(GadgetAttack) ; true), 
    (var(Gender) -> questionGender(Gender) ; true),
    (var(Attack) -> questionAttack(Attack) ; true),
    (var(ShortName) -> questionShortName(ShortName) ; true),
    (var(SuperDamage) -> questionSuperDamage(SuperDamage) ; true),
    write("*Введите имя нового персонажа"), nl,
    read_term(Char, [var_prefix]),
    
    (exists(Rarity, Race, Speed, GadgetAttack, Gender, Attack, ShortName, SuperDamage) -> write("-Персонаж с такими данными уже существует!"), nl, !, fail; true),

    assertz(rarity(Char, Rarity)), assertz(race(Char, Race)), assertz(speed(Char, Speed)), assertz(gadget_attack(Char, GadgetAttack)),
    assertz(gender(Char, Gender)), assertz(attack(Char, Attack)), assertz(short_name(Char, ShortName)), assertz(super_damage(Char, SuperDamage)),

    append('bsa.txt'),
    write(Char), nl,
    write(Rarity), write("."), nl,
    write(Race), write("."), nl,
    write(Speed), write("."), nl,
    write(GadgetAttack), write("."), nl,
    write(Gender), write("."), nl,
    write(Attack), write("."), nl,
    write(ShortName), write("."), nl,
    write(SuperDamage), write("."), nl,
    told,
    
    write("+Персонаж успешно добавлен!"), nl.


main :- 
    set_prolog_flag(encoding, utf8), see('bsa.txt'), read_brawlers, seen, find.

NAME wrk1 VERSION 2
    CONCURENCY 1
    DESCRIPTION                 'workflow de test'
    MATCHING (Country = 'France')

    DEFINE EVENT     Event1                     'incoming event 1';
    DEFINE EVENT     Event2                     'incoming event 2';

    DEFINE RULE      IsMajor (INTEGER agemin)   'this method 1';
    DEFINE RULE      IsEmpty (TEXT text)        'this method 2'; 

    DEFINE ACTION    Cut(TEXT key)              'Remove user';

    DEFINE CONST     Name 'gael'                'ben oui c est moi';
    DEFINE CONST     agemin 18                  'min for been major';

    INITIALIZE WORKFLOW
        ON EVENT Event1 WHEN (NOT IsEmpty(text = @Event.ExternalId)) 
            RECURSIVE SWITCH State1

    DEFINE STATE State1                         'state 1'
        ON ENTER STATE 
            WHEN IsMajor(agemin = agemin)
                EXECUTE Cut(key = @Event.ExternalId)
                     -- Cut(key = @Event.ExternalId)
                STORE   (Status = 'InProgress')           
                     -- (Status = 'InProgress')           

        ON EVENT Event1
            SWITCH State2 

        EXPIRE AFTER 2 DAY
            SWITCH State3

    ;

    DEFINE STATE State2            'state 2'

    ;

    DEFINE STATE State3            'state 3'
    
    ;